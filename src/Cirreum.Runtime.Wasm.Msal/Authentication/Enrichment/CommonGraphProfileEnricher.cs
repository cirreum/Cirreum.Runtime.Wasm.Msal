namespace Cirreum.Runtime.Authentication.Enrichment;

using Cirreum.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using System.Globalization;
using System.Security.Claims;

/// <summary>
/// Base class for Microsoft Graph profile enrichers that provides common functionality
/// for retrieving and processing user data from Microsoft Graph.
/// </summary>
/// <param name="logger">The logger instance for recording diagnostic information.</param>
/// <param name="clock">The clock service used for date, time and timezone handling.</param>
public abstract class CommonGraphProfileEnricher(
	ILogger logger,
	IDateTimeClock clock
) : IUserProfileEnricher {

	/// <summary>
	/// Standard set of fields to retrieve for directory objects: ID and display name.
	/// </summary>
	protected static readonly string[] IdAndDisplayName = [
		"id",
		"displayName"
	];

	/// <summary>
	/// Standard profile picture size to request from Microsoft Graph.
	/// </summary>
	protected static readonly string ProfilePictureSize = "96x96";

	/// <summary>
	/// Enriches a user profile with data from Microsoft Graph based on the user's identity claims.
	/// </summary>
	/// <param name="profile">The user profile to enrich with additional data.</param>
	/// <param name="identity">The claims identity containing authentication information.</param>
	/// <returns>A task representing the asynchronous enrichment operation.</returns>
	/// <exception cref="ArgumentNullException">Thrown if profile or identity is null.</exception>
	public async Task EnrichProfileAsync(UserProfile profile, ClaimsIdentity identity) {
		ArgumentNullException.ThrowIfNull(profile, nameof(profile));
		ArgumentNullException.ThrowIfNull(identity, nameof(identity));

		logger.EnrichingProfile(this.GetType().Name);

		//
		// Process Common claims
		//
#if DEBUG
		const bool captureUnknownClaims = true;
#else
		const bool captureUnknownClaims = false;
#endif
		ClaimsUserProfileEnricher.EnrichProfile(profile, identity, captureUnknownClaims);

		//
		// Process MSAL claims
		//
		var tid = identity.GetTenantId();
		if (tid.IsEmpty()) {
			logger.MissingTenantId();
			return;
		}
		var oid = identity.GetObjectId();
		if (oid.IsEmpty()) {
			logger.MissingObjectId();
			return;
		}
		profile.UpdatedAt = identity.UpdatedAt();
		profile.EmailVerified = identity.IsEmailVerified();
		profile.PhoneNumberVerified = identity.IsPhoneNumberVerified();


		//
		// Query the Graph
		//
		var graphUser = new User();
		var mailSettings = new MailboxSettings();
		List<Organization> organizations = [];
		List<DirectoryObject> directoryMemberships = [];
		string? profilePictureUrl = null;

		try {
			(graphUser, mailSettings, organizations, directoryMemberships, profilePictureUrl)
				= await this.QueryGraph();
		} catch (ODataError ode) {
			logger.ODataError(ode);
		} catch (ServiceException se) {
			logger.GraphServiceError(se);
		} catch (Exception e) {
			logger.UnknownGraphError(e);
		}

		//
		// Process Graph data
		//

		// Organization
		var profileOrg = UserProfileOrganization.Empty;
		if (organizations.Count > 0) {
			var org = organizations[0];
			profileOrg = new UserProfileOrganization {
				OrganizationId = org.Id ?? tid ?? "",
				OrganizationName = org.DisplayName,
			};
		} else {
			profileOrg = new UserProfileOrganization {
				OrganizationId = tid ?? "",
			};
		}

		// Organization Memberships
		var memberships = ProcessMemberships(directoryMemberships);
		profileOrg.DirectoryRoles.AddRange(memberships.Where(m => m.Type == UserProfileMembershipType.DirectoryRole));
		profileOrg.DirectoryGroups.AddRange(memberships.Where(m => m.Type == UserProfileMembershipType.DirectoryGroup));


		//
		// Enrich Profile
		//

		// Core Profile
		profile.GivenName = graphUser.GivenName;
		profile.FamilyName = graphUser.Surname;
		profile.Nickname = graphUser.MailNickname;
		profile.Birthdate = (graphUser.Birthday ?? DateTimeOffset.MinValue).ToString();
		profile.Email = graphUser.Mail;
		profile.PhoneNumber = graphUser.MobilePhone;
		profile.PhoneNumbers = graphUser.BusinessPhones ?? [];
		profile.Locale = graphUser.PreferredLanguage ?? CultureInfo.CurrentUICulture.Name;
		profile.JobTitle = graphUser.JobTitle;
		profile.Company = graphUser.CompanyName;
		profile.OfficeLocation = graphUser.OfficeLocation;
		profile.Department = graphUser.Department;
		profile.EmployeeId = graphUser.EmployeeId;
		profile.EmployeeType = graphUser.EmployeeType;
		profile.Address = new UserProfileAddress {
			StreetAddress = graphUser.StreetAddress,
			City = graphUser.City,
			State = graphUser.State,
			PostalCode = graphUser.PostalCode,
			Country = graphUser.Country
		};

		// EntraID specific
		profile.Oid = oid;
		profile.Upn = graphUser.UserPrincipalName;
		profile.DisplayName = graphUser.DisplayName;

		// Photos
		profile.Picture = profilePictureUrl ?? "/assets/images/guest-user-icon.svg";

		// Organization
		profile.Organization = profileOrg;

		// MailboxSettings
		profile.TimeZone = mailSettings.TimeZone ?? clock.LocalTimeZoneId;
		profile.DateFormat = mailSettings.DateFormat ?? CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;
		profile.TimeFormat = mailSettings.TimeFormat ?? CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern;

		// Update timestamp
		profile.CreatedAt = graphUser.CreatedDateTime ?? clock.LocalOffset;

		// Any custom attributes from Graph
		profile.AdditionalData = GetAdditionalData(graphUser);


	}

	/// <summary>
	/// Queries Microsoft Graph API to retrieve user data.
	/// </summary>
	/// <returns>A <see cref="CommonGraphData"/> object containing the retrieved user information.</returns>
	protected abstract Task<CommonGraphData> QueryGraph();

	/// <summary>
	/// Extracts custom attributes from a Graph user object.
	/// </summary>
	/// <param name="graphUser">The Microsoft Graph user object.</param>
	/// <returns>A dictionary containing any custom extension attributes for the user.</returns>
	private static Dictionary<string, string> GetAdditionalData(User graphUser) {

		var customAttributes = new Dictionary<string, string>();

		if (graphUser.AdditionalData is { } additionalData) {
			foreach (var prop in additionalData) {
				if (prop.Key.StartsWith("extension_")) {
					customAttributes[prop.Key] = $"{prop.Value}";
				}
			}
		}

		return customAttributes;

	}

	/// <summary>
	/// Processes directory objects to extract membership information for the user profile.
	/// </summary>
	/// <param name="directoryObjects">List of directory objects representing user memberships.</param>
	/// <returns>A list of user profile memberships containing roles and groups.</returns>
	private static List<UserProfileMembership> ProcessMemberships(List<DirectoryObject> directoryObjects) {
		var memberships = new List<UserProfileMembership>();

		foreach (var role in directoryObjects) {
			if (role is Group directoryGroup) {
				if (directoryGroup.Id.HasValue()) {
					memberships.Add(new UserProfileMembership(
						directoryGroup.Id,
						$"{directoryGroup.DisplayName}",
						UserProfileMembershipType.DirectoryGroup
					));
				}
			} else if (role is DirectoryRole directoryRole) {
				if (directoryRole.Id.HasValue()) {
					memberships.Add(new UserProfileMembership(
						directoryRole.Id,
						$"{directoryRole.DisplayName}",
						UserProfileMembershipType.DirectoryRole
					));
				}
			}
		}

		return memberships;

	}

	/// <summary>
	/// Converts the contents of a photo stream to a PNG data URI string encoded in Base64.
	/// </summary>
	/// <remarks>The method reads the entire stream from its current position to the end. The caller is responsible
	/// for ensuring the stream is positioned correctly before calling this method.</remarks>
	/// <param name="photoStream">The stream containing the photo data to convert. Can be null.</param>
	/// <returns>A string containing the data URI representation of the photo in PNG format, or null if <paramref
	/// name="photoStream"/> is null.</returns>
	protected static async Task<string?> ConvertPhotoToDataUri(Stream? photoStream) {
		if (photoStream is not null) {
			using var memoryStream = new MemoryStream();
			await photoStream.CopyToAsync(memoryStream);
			var photoBytes = memoryStream.ToArray();
			return $"data:image/png;base64,{Convert.ToBase64String(photoBytes)}";
		}
		return null;
	}

}