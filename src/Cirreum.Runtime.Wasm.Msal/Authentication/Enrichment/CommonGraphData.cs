namespace Cirreum.Runtime.Authentication.Enrichment;

using Microsoft.Graph.Models;

/// <summary>
/// Represents common user data retrieved from Microsoft Graph API.
/// </summary>
/// <param name="GraphUser">The user information from Microsoft Graph.</param>
/// <param name="MailboxSettings">User's mailbox configuration settings.</param>
/// <param name="Organizations">List of organizations the user belongs to.</param>
/// <param name="Memberships">List of directory objects representing group or role memberships.</param>
/// <param name="ProfilePictureUrl">URL to the user's profile picture, or null if unavailable.</param>
public record CommonGraphData(
	User GraphUser,
	MailboxSettings MailboxSettings,
	List<Organization> Organizations,
	List<DirectoryObject> Memberships,
	string? ProfilePictureUrl
);