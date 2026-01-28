namespace Cirreum.Runtime.Authentication.Enrichment;

using System.Security.Claims;

/// <summary>
/// Provides extension methods for extracting and processing Microsoft Graph-related claims
/// from a <see cref="ClaimsIdentity"/>.
/// </summary>
internal static class GraphClaimsHelper {

	/// <summary>
	/// Gets the tenant ID ("tid") claim value from the identity.
	/// </summary>
	/// <param name="identity">The claims identity to extract from.</param>
	/// <returns>The tenant ID value, or null if not present.</returns>
	public static string? GetTenantId(this ClaimsIdentity identity) {
		return identity.FindFirst("tid")?.Value;
	}

	/// <summary>
	/// Gets the object ID ("oid") claim value from the identity.
	/// </summary>
	/// <param name="identity">The claims identity to extract from.</param>
	/// <returns>The object ID value, or null if not present.</returns>
	public static string? GetObjectId(this ClaimsIdentity identity) {
		return identity.FindFirst("oid")?.Value;
	}

	/// <summary>
	/// Determines if the user's email is verified based on claims.
	/// </summary>
	/// <param name="identity">The claims identity to check.</param>
	/// <returns>True if email is verified, false if not verified, or null if the information is not available.</returns>
	public static bool? IsEmailVerified(this ClaimsIdentity identity) {
		var emailClaimValue = identity.FindFirst("email")?.Value;
		var xmsEdovClaimValue = identity.FindFirst("xms_edov")?.Value;
		return (emailClaimValue.HasValue() && xmsEdovClaimValue.HasValue()) && bool.TryParse(xmsEdovClaimValue, out var v)
			? v
			: null;
	}

	/// <summary>
	/// Determines if the user's phone number is verified based on claims.
	/// </summary>
	/// <param name="identity">The claims identity to check.</param>
	/// <returns>True if phone number is verified, false if not verified, or null if the information is not available.</returns>
	public static bool? IsPhoneNumberVerified(this ClaimsIdentity identity) {
		var pnv = identity.FindFirst("phonenumber_verified")?.Value;
		return pnv.HasValue() && bool.TryParse(pnv, out var v) ? v : null;
	}

	/// <summary>
	/// Gets the last update timestamp for the user profile from claims.
	/// </summary>
	/// <param name="identity">The claims identity to extract from.</param>
	/// <returns>The update timestamp, or DateTimeOffset.MinValue if not available or not parseable.</returns>
	public static DateTimeOffset UpdatedAt(this ClaimsIdentity identity) {
		var claim = identity.FindFirst("updated_at")?.Value;
		return claim.HasValue() && DateTimeOffset.TryParse(claim, out var v)
			? v
			: DateTimeOffset.MinValue;
	}

}