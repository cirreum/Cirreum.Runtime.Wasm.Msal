namespace Cirreum.Runtime.Authentication;

using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Diagnostics;
using System.Text.Json.Serialization;

/// <summary>
/// Represents a user account from Microsoft Entra ID (formerly Azure Active Directory)
/// for use with MSAL authentication in Blazor WebAssembly applications.
/// </summary>
/// <remarks>
/// This class extends <see cref="RemoteUserAccount"/> to support Entra-specific
/// role management by providing Roles property.
/// </remarks>
[DebuggerDisplay("{AdditionalProperties[\"name\"] ?? AdditionalProperties[\"email\"] ?? \"Unknown User\"} ({Roles.Count} roles)")]
public sealed class EntraUserAccount : RemoteUserAccount {
	/// <summary>
	/// Gets or sets the list of roles assigned to the user in Entra ID.
	/// </summary>
	/// <remarks>
	/// These roles are typically assigned through Entra ID groups or app roles
	/// and are included in the JWT token claims during authentication.
	/// </remarks>
	[JsonPropertyName("roles")]
	public List<string> Roles { get; set; } = [];
}