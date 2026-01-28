namespace Cirreum.Runtime.Authentication;

using Cirreum.Runtime.Authentication.Enrichment;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring Graph-based user profile enrichment and presence services.
/// </summary>
public static class GraphEnabledBuilderExtensions {

	internal static readonly List<string> MinimalGraphScopes = [
		"User.Read"
	];

	internal static readonly List<string> ExtendedGraphScopes = [
		"User.Read",
		"MailboxSettings.Read",
		"Directory.AccessAsUser.All"
	];

	/// <summary>
	/// Configures basic user profile enrichment using Microsoft Graph.
	/// </summary>
	/// <param name="builder">The Graph-enabled builder.</param>
	/// <returns>The builder for method chaining.</returns>
	/// <remarks>
	/// <para>
	/// This configuration:
	/// <list type="bullet">
	///     <item>
	///         <description>Uses the minimal User.Read scope</description>
	///     </item>
	///     <item>
	///         <description>Registers GraphMinimalUserProfileEnricher for basic profile data</description>
	///     </item>
	/// </list>
	/// </para>
	/// </remarks>
	public static IGraphEnabledBuilder WithMinimalGraphEnrichment(this IGraphEnabledBuilder builder) {
		builder.Services.AddSingleton(sp => new GraphAuthenticationOptions() { RequiredScopes = MinimalGraphScopes });
		builder.Enrichment.WithEnricher<GraphMinimalUserProfileEnricher>();
		return builder;
	}

	/// <summary>
	/// Configures extended user profile enrichment using Microsoft Graph.
	/// </summary>
	/// <param name="builder">The Graph-enabled builder.</param>
	/// <param name="graphScopes">Optional custom scopes. Defaults to User.Read, MailboxSettings.Read, and Directory.AccessAsUser.All.</param>
	/// <returns>The builder for method chaining.</returns>
	/// <remarks>
	/// <para>
	/// This configuration:
	/// <list type="bullet">
	///     <item>
	///         <description>Uses extended Graph scopes for additional profile data</description>
	///     </item>
	///     <item>
	///         <description>Registers GraphExtendedUserProfileEnricher for comprehensive profile enrichment</description>
	///     </item>
	/// </list>
	/// </para>
	/// </remarks>
	public static IGraphEnabledBuilder WithExtendedGraphEnrichment(this IGraphEnabledBuilder builder, List<string>? graphScopes = null) {
		var privateGraphScopes = graphScopes?.Count > 0 ? graphScopes : ExtendedGraphScopes;
		builder.Services.AddSingleton(sp => new GraphAuthenticationOptions { RequiredScopes = privateGraphScopes });
		builder.Enrichment.WithEnricher<GraphExtendedUserProfileEnricher>();
		return builder;
	}

	/// <summary>
	/// Configures user profile enrichment for external tenants using Microsoft Graph.
	/// </summary>
	/// <param name="builder">The external Graph-enabled builder.</param>
	/// <returns>The builder for method chaining.</returns>
	/// <remarks>
	/// <para>
	/// This configuration:
	/// <list type="bullet">
	///     <item>
	///         <description>Uses the minimal User.Read scope</description>
	///     </item>
	///     <item>
	///         <description>Registers ExternalGraphUserProfileEnricher for external tenant profile data</description>
	///     </item>
	/// </list>
	/// </para>
	/// </remarks>
	public static IExternalGraphEnabledBuilder WithGraphEnrichment(this IExternalGraphEnabledBuilder builder) {
		builder.Services.AddSingleton(sp => new GraphAuthenticationOptions() { RequiredScopes = MinimalGraphScopes });
		builder.Enrichment.WithEnricher<ExternalGraphUserProfileEnricher>();
		return builder;
	}

}