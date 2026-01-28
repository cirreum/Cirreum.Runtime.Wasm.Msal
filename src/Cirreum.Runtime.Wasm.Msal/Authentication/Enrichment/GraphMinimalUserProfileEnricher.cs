namespace Cirreum.Runtime.Authentication.Enrichment;

using Cirreum;
using Cirreum.Graph.Provider;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;

/// <summary>
/// Enriches the <see cref="UserProfile"/> with minimal information from Microsoft Graph,
/// optimized for performance and reduced API usage.
/// </summary>
/// <remarks>
/// This enricher retrieves basic user data, mailbox settings, and organization information,
/// but deliberately skips directory memberships to minimize API calls and processing.
/// </remarks>
/// <param name="provider">The <see cref="IGraphServiceClientProvider"/> that exposes the Microsoft Graph client.</param>
/// <param name="logger">The logger instance for recording diagnostic information.</param>
/// <param name="clock">The clock service used for date, time and timezone handling.</param>
public sealed class GraphMinimalUserProfileEnricher(
	IGraphServiceClientProvider provider,
	ILogger<GraphMinimalUserProfileEnricher> logger,
	IDateTimeClock clock
) : CommonGraphProfileEnricher(logger, clock) {

	/// <summary>
	/// The minimal set of user fields requested from Microsoft Graph.
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
		"createdDateTime"
	];

	/// <summary>
	/// Queries Microsoft Graph API to retrieve minimal user data using batch requests.
	/// </summary>
	/// <returns>A <see cref="CommonGraphData"/> object containing the basic set of retrieved user information.</returns>
	/// <remarks>
	/// For minimal enrichment, directory memberships are intentionally skipped to reduce API overhead.
	/// </remarks>
	protected override async Task<CommonGraphData> QueryGraph() {

		//
		// Create the Batch request
		//
		return await provider.UseClientAsync(async graphClient => {

			var batch = new BatchRequestContentCollection(graphClient);
			var userRequestId = await batch.AddBatchRequestStepAsync(graphClient.Me.ToGetRequestInformation(requestConfiguration => requestConfiguration.QueryParameters.Select = UserQuery));
			var mailboxRequestId = await batch.AddBatchRequestStepAsync(graphClient.Me.MailboxSettings.ToGetRequestInformation());
			var orgRequestId = await batch.AddBatchRequestStepAsync(graphClient.Organization.ToGetRequestInformation(requestConfiguration => requestConfiguration.QueryParameters.Select = IdAndDisplayName));
			// For minimal enrichment, we skip directory memberships

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

			// For minimal enrichment, we skip directory memberships
			var memberships = new List<DirectoryObject>();

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