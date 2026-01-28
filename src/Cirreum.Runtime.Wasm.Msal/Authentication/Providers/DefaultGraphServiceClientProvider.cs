namespace Cirreum.Runtime.Authentication.Providers;

using Cirreum.Graph.Provider;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;

internal sealed class DefaultGraphServiceClientProvider : IGraphServiceClientProvider {

	internal static readonly string HttpClientName = "_GraphHttpClient";
	internal static readonly string GraphUrl = "https://graph.microsoft.com/v1.0";

	private readonly List<string> _scopes;
	private readonly HttpClient _httpClient;
	private readonly IAccessTokenProvider _tokenProvider;
	private GraphServiceClient? _client;

	public DefaultGraphServiceClientProvider(
		IServiceProvider serviceProvider,
		IAccessTokenProvider tokenProvider) {

		this._tokenProvider = tokenProvider;
		this._scopes = serviceProvider.GetService<GraphAuthenticationOptions>()?.RequiredScopes ??
			GraphEnabledBuilderExtensions.MinimalGraphScopes;

		var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
		this._httpClient =
			httpClientFactory?.CreateClient(HttpClientName)
			?? new HttpClient { BaseAddress = new Uri(GraphUrl) };

	}

	public async Task<T> UseClientAsync<T>(Func<GraphServiceClient, Task<T>> action)
		=> await action(this.GetClient());

	public async Task UseClientAsync(Func<GraphServiceClient, Task> action)
		=> await action(this.GetClient());

	private GraphServiceClient GetClient() {
		return this._client ??= new GraphServiceClient(
			this._httpClient,
			new GraphAuthenticationProvider(this._tokenProvider, this._scopes),
			GraphUrl);
	}

}