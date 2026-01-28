namespace Cirreum.Runtime.Authentication;

using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

internal sealed class MsalClaimsPrincipalFactory(
	IAccessTokenProviderAccessor accessor,
	IServiceProvider serviceProvider,
	ILogger<MsalClaimsPrincipalFactory> logger,
	IEnumerable<IClaimsExtender>? claimsExtenders = null,
	IEnumerable<IAuthenticationPostProcessor>? postProcessors = null
) : CommonClaimsPrincipalFactory<EntraUserAccount>(
		logger,
		serviceProvider,
		accessor,
		claimsExtenders,
		postProcessors) {

	protected override ValueTask MapIdentityAsync(ClaimsIdentity identity, EntraUserAccount account) {

		// Add a claim for each role
		account.Roles.ForEach((role) => {
			identity.AddClaim(new Claim("roles", role));
		});

		return default;
	}

}