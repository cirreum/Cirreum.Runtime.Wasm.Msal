namespace Cirreum.Runtime.Tests.Authentication.Enrichment;

using Cirreum.Runtime.Authentication.Enrichment;
using FluentAssertions;
using Microsoft.Graph.Models;
using Xunit;

public class CommonGraphDataTests {

	[Fact]
	public void Constructor_SetsAllProperties() {
		// Arrange
		var user = new User { DisplayName = "Test User" };
		var mailboxSettings = new MailboxSettings { TimeZone = "UTC" };
		var organizations = new List<Organization> { new() { DisplayName = "Test Org" } };
		var memberships = new List<DirectoryObject> { new Group { DisplayName = "Test Group" } };
		var profilePictureUrl = "https://example.com/photo.jpg";

		// Act
		var data = new CommonGraphData(user, mailboxSettings, organizations, memberships, profilePictureUrl);

		// Assert
		data.GraphUser.Should().BeSameAs(user);
		data.MailboxSettings.Should().BeSameAs(mailboxSettings);
		data.Organizations.Should().BeSameAs(organizations);
		data.Memberships.Should().BeSameAs(memberships);
		data.ProfilePictureUrl.Should().Be(profilePictureUrl);
	}

	[Fact]
	public void ProfilePictureUrl_CanBeNull() {
		// Arrange
		var user = new User();
		var mailboxSettings = new MailboxSettings();
		var organizations = new List<Organization>();
		var memberships = new List<DirectoryObject>();

		// Act
		var data = new CommonGraphData(user, mailboxSettings, organizations, memberships, null);

		// Assert
		data.ProfilePictureUrl.Should().BeNull();
	}

	[Fact]
	public void Record_SupportsDeconstruction() {
		// Arrange
		var user = new User { DisplayName = "Test" };
		var mailboxSettings = new MailboxSettings();
		var organizations = new List<Organization>();
		var memberships = new List<DirectoryObject>();
		var photoUrl = "https://test.com/photo";
		var data = new CommonGraphData(user, mailboxSettings, organizations, memberships, photoUrl);

		// Act
		var (graphUser, mailbox, orgs, members, picture) = data;

		// Assert
		graphUser.Should().BeSameAs(user);
		mailbox.Should().BeSameAs(mailboxSettings);
		orgs.Should().BeSameAs(organizations);
		members.Should().BeSameAs(memberships);
		picture.Should().Be(photoUrl);
	}

	[Fact]
	public void Record_SupportsEquality() {
		// Arrange
		var user = new User();
		var mailboxSettings = new MailboxSettings();
		var organizations = new List<Organization>();
		var memberships = new List<DirectoryObject>();

		var data1 = new CommonGraphData(user, mailboxSettings, organizations, memberships, "photo");
		var data2 = new CommonGraphData(user, mailboxSettings, organizations, memberships, "photo");

		// Assert
		data1.Should().Be(data2);
	}

	[Fact]
	public void Record_WithDifferentPhoto_IsNotEqual() {
		// Arrange
		var user = new User();
		var mailboxSettings = new MailboxSettings();
		var organizations = new List<Organization>();
		var memberships = new List<DirectoryObject>();

		var data1 = new CommonGraphData(user, mailboxSettings, organizations, memberships, "photo1");
		var data2 = new CommonGraphData(user, mailboxSettings, organizations, memberships, "photo2");

		// Assert
		data1.Should().NotBe(data2);
	}

}
