namespace Cirreum.Runtime.Authentication.Enrichment;

using Cirreum;
using Cirreum.Graph.Provider;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;

/// <summary>
/// Fully enriches the <see cref="UserProfile"/> with comprehensive information from Microsoft Graph,
/// including detailed user data, mailbox settings, organization information, directory memberships,
/// and profile picture.
/// </summary>
/// <remarks>
/// This enricher uses batch requests to efficiently retrieve multiple resource types in a single
/// call to Microsoft Graph, reducing API overhead.
/// </remarks>
/// <param name="provider">The <see cref="IGraphServiceClientProvider"/> that exposes the Microsoft Graph client.</param>
/// <param name="logger">The logger instance for recording diagnostic information.</param>
/// <param name="clock">The clock service used for date, time and time zone handling.</param>
public class GraphExtendedUserProfileEnricher(
	IGraphServiceClientProvider provider,
	ILogger<GraphExtendedUserProfileEnricher> logger,
	IDateTimeClock clock
) : CommonGraphProfileEnricher(logger, clock) {

	/// <summary>
	/// The comprehensive set of user fields requested from Microsoft Graph.
	/// </summary>
	private static readonly string[] UserQuery = [
		"userPrincipalName",
		"displayName",
		"givenName",
		"surname",
		"mailNickname",
		"birthday",
		"mail",
		"mobilePhone",
		"businessPhones",
		"preferredLanguage",
		"jobTitle",
		"companyName",
		"officeLocation",
		"department",
		"employeeId",
		"employeeType",
		"streetAddress",
		"city",
		"state",
		"postalCode",
		"country",
		"createdDateTime"
	];

	/// <summary>
	/// Queries Microsoft Graph API to retrieve comprehensive user data using batch requests.
	/// </summary>
	/// <returns>A <see cref="CommonGraphData"/> object containing the full set of retrieved user information.</returns>
	protected override async Task<CommonGraphData> QueryGraph() {

		//
		// Create the Batch request
		//
		return await provider.UseClientAsync(async graphClient => {

			var batch = new BatchRequestContentCollection(graphClient);
			var userRequestId = await batch.AddBatchRequestStepAsync(graphClient.Me.ToGetRequestInformation(requestConfiguration => requestConfiguration.QueryParameters.Select = UserQuery));
			var mailboxRequestId = await batch.AddBatchRequestStepAsync(graphClient.Me.MailboxSettings.ToGetRequestInformation());
			var orgRequestId = await batch.AddBatchRequestStepAsync(graphClient.Organization.ToGetRequestInformation(requestConfiguration => requestConfiguration.QueryParameters.Select = IdAndDisplayName));
			var requestMemberOfId = await batch.AddBatchRequestStepAsync(graphClient.Me.MemberOf.ToGetRequestInformation(requestConfiguration => requestConfiguration.QueryParameters.Select = IdAndDisplayName));


			//
			// Query the Graph...
			//

			//batch query
			var batchResponse = await graphClient.Batch.PostAsync(batch);

			// photo query
			var photoStream = await graphClient.Me.Photos[ProfilePictureSize].Content.GetAsync();

			//
			// Process results...
			//

			var graphUser = await batchResponse.GetResponseByIdAsync<User>(userRequestId) ?? new User();

			var mailSetting = await batchResponse.GetResponseByIdAsync<MailboxSettings>(mailboxRequestId);

			var organizations = new List<Organization>();
			var orgs = await batchResponse.GetResponseByIdAsync<OrganizationCollectionResponse>(orgRequestId);
			if (orgs != null && orgs.Value is not null) {
				organizations = orgs.Value;
			}

			var memberships = new List<DirectoryObject>();
			var memberOfResponse = await batchResponse.GetResponseByIdAsync<DirectoryObjectCollectionResponse>(requestMemberOfId);
			if (memberOfResponse?.Value is { Count: > 0 }) {
				memberships = memberOfResponse.Value;
			}

			// Convert photo to data URI
			var photoUri = await ConvertPhotoToDataUri(photoStream);

			//
			// Return the data
			//
			return new CommonGraphData(
				graphUser,
				mailSetting,
				organizations,
				memberships,
				photoUri);

		});

	}

}