namespace Cirreum.Runtime.Authentication;

using Cirreum.Authorization;
using Cirreum.Graph.Provider;
using Cirreum.Presence;
using Cirreum.Runtime;
using Cirreum.Runtime.Authentication.Builders;
using Cirreum.Runtime.Authentication.Providers;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring additional authentication related services in a Blazor WebAssembly application.
/// </summary>
public static class EntraAuthenticationExtensions {

	//
	// Claims Extender
	//

	/// <summary>
	/// Adds a claims extender to the authentication services for workforce tenants.
	/// </summary>
	/// <typeparam name="TClaimsExtender">The type of the claims extender to add. Must implement
	/// <see cref="IClaimsExtender"/>.</typeparam>
	/// <param name="builder">The authentication services builder.</param>
	/// <returns>The same <see cref="IEntraAuthenticationBuilder"/> instance for method chaining.</returns>
	/// <remarks>
	/// Claims extenders allow for customization of user claims after authentication, enabling additional
	/// claim transformations before user profile enrichment occurs.
	/// </remarks>
	public static IEntraAuthenticationBuilder AddClaimsExtender<TClaimsExtender>(
		this IEntraAuthenticationBuilder builder)
		where TClaimsExtender : class, IClaimsExtender {
		builder.Services.AddScoped<IClaimsExtender, TClaimsExtender>();
		return builder;
	}

	/// <summary>
	/// Adds a claims extender to the authentication services for external tenants.
	/// </summary>
	/// <typeparam name="TClaimsExtender">The type of the claims extender to add. Must implement 
	/// <see cref="IClaimsExtender"/>.</typeparam>
	/// <param name="builder">The external authentication services builder.</param>
	/// <returns>The same <see cref="IEntraExternalBuilder"/> instance for method chaining.</returns>
	/// <remarks>
	/// Claims extenders allow for customization of user claims after authentication, enabling additional
	/// claim transformations before user profile enrichment occurs.
	/// </remarks>
	public static IEntraExternalBuilder AddClaimsExtender<TClaimsExtender>(
		this IEntraExternalBuilder builder)
		where TClaimsExtender : class, IClaimsExtender {
		builder.Builder.AddClaimsExtender<TClaimsExtender>();
		return builder;
	}


	//
	// GraphServices
	//

	/// <summary>
	/// Configures core Microsoft Graph services and authentication for workforce tenants.
	/// </summary>
	/// <param name="builder">The Graph service builder.</param>
	/// <returns>An <see cref="IGraphEnabledBuilder"/> configured with IGraphServiceClientProvider, profile enrichment, and presence capabilities.</returns>
	/// <remarks>
	/// This method configures:
	/// <para>IGraphServiceClientProvider with token-based authentication</para>
	/// <para>User profile enrichment services</para>
	/// <para>User presence tracking capabilities</para>
	/// </remarks>
	public static IGraphEnabledBuilder AddGraphServices(
		this IEntraAuthenticationBuilder builder) {

		builder.Services.AddHttpClient(DefaultGraphServiceClientProvider.HttpClientName, client => {
			client.BaseAddress = new Uri(DefaultGraphServiceClientProvider.GraphUrl);
		});

		builder.Services.AddScoped<IGraphServiceClientProvider, DefaultGraphServiceClientProvider>();

		return new GraphEnabledBuilder(
			builder.Services,
			builder,
			new UserPresenceBuilder(builder.Services));

	}

	/// <summary>
	/// Configures core Microsoft Graph services and authentication for external tenants.
	/// </summary>
	/// <param name="builder">The external Graph service builder.</param>
	/// <returns>An <see cref="IExternalGraphEnabledBuilder"/> for configuring external tenant Graph services.</returns>
	/// <remarks>
	/// This method configures Microsoft Graph services for external tenants with appropriate scopes and
	/// authentication mechanisms.
	/// </remarks>
	public static IExternalGraphEnabledBuilder AddGraphServices(
		this IEntraExternalBuilder builder) {
		return new ExternalGraphEnabledBuilder(builder.Builder.AddGraphServices());
	}



	//
	// Session Monitoring
	//

	/// <summary>
	/// Adds session monitoring services.
	/// </summary>
	/// <param name="builder">The <see cref="IEntraAuthenticationBuilder"/> to which session monitoring is added.</param>
	/// <param name="configure">An optional delegate to configure the <see cref="SessionOptions"/>. If not provided, default options will be used.</param>
	/// <returns>The <see cref="IEntraAuthenticationBuilder"/> instance, enabling further configuration of the authentication pipeline.</returns>
	/// <remarks>
	/// <para>
	/// This method registers the necessary services for monitoring session activity, including HTTP
	/// monitoring and session-specific configuration. By default, session monitoring is enabled, but the
	/// behavior can be customized using the <paramref name="configure"/> delegate. HTTP monitoring is only
	/// registered when <see cref="SessionOptions.TrackApiCalls"/> is enabled to avoid unnecessary overhead.
	/// </para>
	/// </remarks>
	public static IEntraAuthenticationBuilder AddSessionMonitoring(
		this IEntraAuthenticationBuilder builder,
		Action<SessionOptions>? configure = null) {
		builder.Services.AddSessionMonitoring(configure);
		return builder;
	}

	/// <summary>
	/// Adds session monitoring services.
	/// </summary>
	/// <param name="builder">The <see cref="IEntraExternalBuilder"/> to which session monitoring is added.</param>
	/// <param name="configure">An optional delegate to configure the <see cref="SessionOptions"/>. If not provided, default options will be used.</param>
	/// <returns>The <see cref="IEntraExternalBuilder"/> instance, enabling further configuration of the authentication pipeline.</returns>
	/// <remarks>
	/// <para>
	/// This method registers the necessary services for monitoring session activity, including HTTP
	/// monitoring and session-specific configuration. By default, session monitoring is enabled, but the
	/// behavior can be customized using the <paramref name="configure"/> delegate. HTTP monitoring is only
	/// registered when <see cref="SessionOptions.TrackApiCalls"/> is enabled to avoid unnecessary overhead.
	/// </para>
	/// </remarks>
	public static IEntraExternalBuilder AddSessionMonitoring(
		this IEntraExternalBuilder builder,
		Action<SessionOptions>? configure = null) {
		builder.Services.AddSessionMonitoring(configure);
		return builder;
	}


	//
	// Application User
	//

	//
	// Application User
	//

	/// <summary>
	/// Registers application user services with the specified user type and loader implementation.
	/// </summary>
	/// <typeparam name="TApplicationUser">The type of the application user that implements <see cref="IApplicationUser"/>.</typeparam>
	/// <typeparam name="TApplicationUserLoader">The type of the application user loader that implements <see cref="IApplicationUserLoader{TApplicationUser}"/>.</typeparam>
	/// <param name="builder">The <see cref="IEntraAuthenticationBuilder"/> to add services to.</param>
	/// <returns>The <see cref="IEntraAuthenticationBuilder"/> so that additional calls can be chained.</returns>
	public static IEntraAuthenticationBuilder AddApplicationUser<TApplicationUser, TApplicationUserLoader>(
		this IEntraAuthenticationBuilder builder)
		where TApplicationUser : class, IApplicationUser
		where TApplicationUserLoader : class, IApplicationUserLoader<TApplicationUser> {

		// Forward to real method
		builder.Services.AddApplicationUser<TApplicationUser, TApplicationUserLoader>();

		return builder;

	}

	/// <summary>
	/// Registers application user services with a custom loader factory function.
	/// </summary>
	/// <typeparam name="TApplicationUser">The type of the application user that implements <see cref="IApplicationUser"/>.</typeparam>
	/// <param name="builder">The <see cref="IEntraAuthenticationBuilder"/> to add services to.</param>
	/// <param name="loaderFactory">A factory function that creates an instance of <see cref="IApplicationUserLoader{TApplicationUser}"/> using the service provider.</param>
	/// <returns>The <see cref="IEntraAuthenticationBuilder"/> so that additional calls can be chained.</returns>
	public static IEntraAuthenticationBuilder AddApplicationUser<TApplicationUser>(
		this IEntraAuthenticationBuilder builder,
		Func<IServiceProvider, IApplicationUserLoader<TApplicationUser>> loaderFactory)
		where TApplicationUser : class, IApplicationUser {

		// Forward to real method
		builder.Services.AddApplicationUser(loaderFactory);

		return builder;

	}


	/// <summary>
	/// Registers application user services with the specified user type and loader implementation.
	/// </summary>
	/// <typeparam name="TApplicationUser">The type of the application user that implements <see cref="IApplicationUser"/>.</typeparam>
	/// <typeparam name="TApplicationUserLoader">The type of the application user loader that implements <see cref="IApplicationUserLoader{TApplicationUser}"/>.</typeparam>
	/// <param name="builder">The <see cref="IEntraExternalBuilder"/> to add services to.</param>
	/// <returns>The <see cref="IEntraExternalBuilder"/> so that additional calls can be chained.</returns>
	public static IEntraExternalBuilder AddApplicationUser<TApplicationUser, TApplicationUserLoader>(
		this IEntraExternalBuilder builder)
		where TApplicationUser : class, IApplicationUser
		where TApplicationUserLoader : class, IApplicationUserLoader<TApplicationUser> {

		// Forward to real method
		builder.Services.AddApplicationUser<TApplicationUser, TApplicationUserLoader>();

		return builder;

	}

	/// <summary>
	/// Registers application user services with a custom loader factory function.
	/// </summary>
	/// <typeparam name="TApplicationUser">The type of the application user that implements <see cref="IApplicationUser"/>.</typeparam>
	/// <param name="builder">The <see cref="IEntraExternalBuilder"/> to add services to.</param>
	/// <param name="loaderFactory">A factory function that creates an instance of <see cref="IApplicationUserLoader{TApplicationUser}"/> using the service provider.</param>
	/// <returns>The <see cref="IEntraExternalBuilder"/> so that additional calls can be chained.</returns>
	public static IEntraExternalBuilder AddApplicationUser<TApplicationUser>(
		this IEntraExternalBuilder builder,
		Func<IServiceProvider, IApplicationUserLoader<TApplicationUser>> loaderFactory)
		where TApplicationUser : class, IApplicationUser {

		// Forward to real method
		builder.Services.AddApplicationUser(loaderFactory);

		return builder;

	}


}