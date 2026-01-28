namespace Cirreum.Runtime.Authentication.Enrichment;

using Cirreum.Graph.Provider;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;

/// <summary>
/// Profile enricher that works with Entra External identities to retrieve a limited set of
/// user profile information from Microsoft Graph.
/// </summary>
/// <remarks>
/// This enricher is designed for external users who may have limited Graph API access
/// and does not attempt to retrieve profile pictures, organization details, or membership information.
/// </remarks>
/// <param name="provider">The <see cref="IGraphServiceClientProvider"/> that exposes the Microsoft Graph client.</param>
/// <param name="logger">The logger instance for recording diagnostic information.</param>
/// <param name="clock">The clock service used for date, time and timezone handling.</param>
public sealed class ExternalGraphUserProfileEnricher(
	IGraphServiceClientProvider provider,
	ILogger<ExternalGraphUserProfileEnricher> logger,
	IDateTimeClock clock
) : CommonGraphProfileEnricher(logger, clock) {

	/// <summary>
	/// The subset of user fields requested from Microsoft Graph for external users.
	/// </summary>
	private static readonly string[] UserQuery = [
		"userPrincipalName",
		"displayName",
		"givenName",
		"surname",
		"mailNickname",
		"mail",
		"mobilePhone",
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
	/// Queries Microsoft Graph API to retrieve user data for external users.
	/// </summary>
	/// <returns>A <see cref="CommonGraphData"/> object containing the retrieved user information,
	/// with limited fields populated for external users.</returns>
	/// <remarks>
	/// Entra External does not support Profile Pictures as of this implementation.
	/// </remarks>
	protected override async Task<CommonGraphData> QueryGraph() {

		var graphUser = await provider.UseClientAsync(c => c.Me.GetAsync(c => c.QueryParameters.Select = UserQuery));

		// photo query - Entra External does not support Profile Pictures as of yet

		return new CommonGraphData(
			graphUser ?? new User(),
			new MailboxSettings(),
			[],
			[],
			null);

	}

}