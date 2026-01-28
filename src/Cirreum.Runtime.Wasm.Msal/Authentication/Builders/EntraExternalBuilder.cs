namespace Cirreum.Runtime.Authentication.Builders;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Builder for external entra services.
/// Provides access to service collection and the underlying Entra authentication builder.
/// </summary>
/// <param name="builder">The underlying Entra authentication builder.</param>
public sealed class EntraExternalBuilder(
	IEntraAuthenticationBuilder builder
) : IEntraExternalBuilder {

	/// <summary>
	/// Gets the service collection.
	/// </summary>
	public IServiceCollection Services => builder.Services;

	/// <summary>
	/// Gets the underlying graph service builder.
	/// </summary>
	public IEntraAuthenticationBuilder Builder => builder;

}