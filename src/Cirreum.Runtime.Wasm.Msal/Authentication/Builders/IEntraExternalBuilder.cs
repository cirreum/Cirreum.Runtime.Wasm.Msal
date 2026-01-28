namespace Cirreum.Runtime.Authentication.Builders;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Interface for additional Entra authentication related services.
/// </summary>
public interface IEntraExternalBuilder {
	/// <summary>
	/// Gets the service collection.
	/// </summary>
	IServiceCollection Services { get; }

	/// <summary>
	/// Gets the underlying authentication services builder.
	/// </summary>
	IEntraAuthenticationBuilder Builder { get; }
}