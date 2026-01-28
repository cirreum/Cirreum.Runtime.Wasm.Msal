namespace Cirreum.Runtime.Authentication;

using Microsoft.Graph;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using IAccessTokenProvider =
	Microsoft.AspNetCore.Components.WebAssembly.Authentication.IAccessTokenProvider;

/// <summary>
/// Provides Microsoft Graph authentication functionality by implementing the Kiota <see cref="IAuthenticationProvider"/> interface.
/// </summary>
/// <remarks>
/// This class handles acquiring and injecting OAuth access tokens into Graph API requests using
/// the ASP.NET Core WebAssembly authentication services. It automatically manages token acquisition
/// using the provided token provider and requested scopes.
/// </remarks>
/// <param name="tokenProvider">The service that provides access tokens for authentication.</param>
/// <param name="scopes">The list of OAuth scopes to request when obtaining access tokens.</param>
internal sealed class GraphAuthenticationProvider(
	IAccessTokenProvider tokenProvider,
	List<string> scopes
) : IAuthenticationProvider {

	/// <summary>
	/// Authenticates a request to the Microsoft Graph API by adding the necessary authentication headers.
	/// </summary>
	/// <param name="request">The request to be authenticated.</param>
	/// <param name="additionalAuthenticationContext">Optional additional context for authentication.</param>
	/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
	/// <returns>A task representing the asynchronous authentication operation.</returns>
	/// <remarks>
	/// This method acquires an access token using the specified scopes and adds it to the request
	/// as a Bearer token in the Authorization header. It also adds an Accept header for JSON responses.
	/// </remarks>
	public async Task AuthenticateRequestAsync(
		RequestInformation request,
		Dictionary<string, object>? additionalAuthenticationContext = null,
		CancellationToken cancellationToken = default) {
		var result = await tokenProvider.RequestAccessToken(new() {
			Scopes = scopes
		});
		if (result.TryGetToken(out var token)) {
			request.Headers.Add("Accept", "application/json");
			request.Headers.Add("Authorization",
				$"{CoreConstants.Headers.Bearer} {token.Value}");
		}
	}
}