namespace Cirreum.Runtime.Authentication.Builders;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Builder for additional authentication related services.
/// </summary>
/// <param name="services">The service collection.</param>
/// <remarks>
/// <para>
/// Provides access to the service collection via the <see cref="Services"/> property.
/// </para>
/// </remarks>
public sealed class EntraAuthenticationBuilder(
	IServiceCollection services
) : IEntraAuthenticationBuilder {
	/// <summary>
	/// Gets the service collection.
	/// </summary>
	public IServiceCollection Services { get; } = services;
}