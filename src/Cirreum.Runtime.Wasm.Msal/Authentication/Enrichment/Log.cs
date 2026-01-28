namespace Cirreum.Runtime.Authentication.Enrichment;

using Microsoft.Extensions.Logging;

/// <summary>
/// Source-generated high-performance logging methods for profile enrichment operations.
/// </summary>
internal static partial class Log {

	[LoggerMessage(
		EventId = 1000,
		Level = LogLevel.Debug,
		Message = "{Enricher} enriching profile from the Microsoft Graph")]
	public static partial void EnrichingProfile(this ILogger logger, string enricher);

	[LoggerMessage(
		EventId = 1001,
		Level = LogLevel.Warning,
		Message = "Unable to get the tid from the ClaimsPrincipal. Ensure the ClaimsPrincipal was created from an Msal/EntraID provider.")]
	public static partial void MissingTenantId(this ILogger logger);

	[LoggerMessage(
		EventId = 1002,
		Level = LogLevel.Warning,
		Message = "Unable to get the oid from the ClaimsPrincipal. Ensure the ClaimsPrincipal was created from an Msal/EntraID provider.")]
	public static partial void MissingObjectId(this ILogger logger);

	[LoggerMessage(
		EventId = 1003,
		Level = LogLevel.Error,
		Message = "OData error querying the Microsoft Graph.")]
	public static partial void ODataError(this ILogger logger, Exception exception);

	[LoggerMessage(
		EventId = 1004,
		Level = LogLevel.Error,
		Message = "Graph Service error querying the Microsoft Graph.")]
	public static partial void GraphServiceError(this ILogger logger, Exception exception);

	[LoggerMessage(
		EventId = 1005,
		Level = LogLevel.Error,
		Message = "Unknown error querying the Microsoft Graph.")]
	public static partial void UnknownGraphError(this ILogger logger, Exception exception);

}
