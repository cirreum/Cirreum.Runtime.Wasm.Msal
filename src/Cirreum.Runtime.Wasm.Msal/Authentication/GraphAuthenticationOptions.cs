namespace Cirreum.Runtime.Authentication;

/// <summary>
/// Configuration options for Microsoft Graph authentication services.
/// </summary>
/// <remarks>
/// This class provides configuration settings for Microsoft Graph authentication,
/// including specifying the required OAuth scopes needed for accessing Graph API resources.
/// </remarks>
public class GraphAuthenticationOptions {
	/// <summary>
	/// Gets or sets the list of OAuth scopes required for Microsoft Graph API access.
	/// </summary>
	/// <remarks>
	/// These scopes determine what level of access the application has to Microsoft Graph resources.
	/// Common scopes include "User.Read", "Mail.Read", "Directory.Read.All", etc.
	/// See the Microsoft Graph documentation for a complete list of available permissions.
	/// </remarks>
	/// <value>
	/// A list of scope string identifiers. Default is an empty list.
	/// </value>
	public List<string> RequiredScopes { get; set; } = [];
}