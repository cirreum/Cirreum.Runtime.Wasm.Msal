namespace Cirreum.Runtime.Authentication.Builders;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Builder for external graph-enabled authentication services.
/// Provides access to service collection and user profile enrichment capabilities.
/// </summary>
/// <param name="builder">The underlying graph-enabled builder.</param>
public sealed class ExternalGraphEnabledBuilder(
	IGraphEnabledBuilder builder
) : IExternalGraphEnabledBuilder {

	/// <summary>
	/// Gets the service collection.
	/// </summary>
	public IServiceCollection Services => builder.Services;

	/// <summary>
	/// Gets the user profile enrichment builder.
	/// </summary>
	public IUserProfileEnrichmentBuilder Enrichment => builder.Enrichment;

}