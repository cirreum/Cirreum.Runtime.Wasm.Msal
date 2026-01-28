namespace Cirreum.Runtime.Authentication.Builders;

using Cirreum;
using Cirreum.Presence;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Builder for graph-enabled authentication services.
/// Provides access to service collection, user profile enrichment, and user presence capabilities.
/// </summary>
/// <param name="services">The service collection.</param>
/// <param name="enrichmentBuilder">The user profile enrichment builder.</param>
/// <param name="presenceBuilder">The user presence builder.</param>
public sealed class GraphEnabledBuilder(
	IServiceCollection services,
	IUserProfileEnrichmentBuilder enrichmentBuilder,
	IUserPresenceBuilder presenceBuilder
) : IGraphEnabledBuilder {

	/// <summary>
	/// Gets the service collection.
	/// </summary>
	public IServiceCollection Services => services;

	/// <summary>
	/// Gets the user profile enrichment builder.
	/// </summary>
	public IUserProfileEnrichmentBuilder Enrichment => enrichmentBuilder;

	/// <summary>
	/// Gets the user presence builder.
	/// </summary>
	public IUserPresenceBuilder Presence => presenceBuilder;

}