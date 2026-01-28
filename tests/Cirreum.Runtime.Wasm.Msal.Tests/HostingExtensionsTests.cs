extern alias MsalLib;

namespace Cirreum.Runtime.Tests;

using FluentAssertions;
using Xunit;

using MsalHostingExtensions = MsalLib::Cirreum.Runtime.HostingExtensions;

public class MsalHostingExtensionsTests {

	[Fact]
	public void DefaultEntraScopes_ContainsExpectedScopes() {
		// Arrange
		var expectedScopes = new HashSet<string> {
			"openid",
			"profile",
			"email",
			"offline_access",
			"User.Read"
		};

		// Act
		var actualScopes = MsalHostingExtensions.DefaultEntraScopes;

		// Assert
		actualScopes.Should().BeEquivalentTo(expectedScopes);
	}

	[Fact]
	public void DefaultEntraScopes_ContainsOpenId() {
		MsalHostingExtensions.DefaultEntraScopes.Should().Contain("openid");
	}

	[Fact]
	public void DefaultEntraScopes_ContainsProfile() {
		MsalHostingExtensions.DefaultEntraScopes.Should().Contain("profile");
	}

	[Fact]
	public void DefaultEntraScopes_ContainsEmail() {
		MsalHostingExtensions.DefaultEntraScopes.Should().Contain("email");
	}

	[Fact]
	public void DefaultEntraScopes_ContainsOfflineAccess() {
		MsalHostingExtensions.DefaultEntraScopes.Should().Contain("offline_access");
	}

	[Fact]
	public void DefaultEntraScopes_ContainsUserRead() {
		MsalHostingExtensions.DefaultEntraScopes.Should().Contain("User.Read");
	}

	[Fact]
	public void DefaultEntraScopes_HasExactlyFiveScopes() {
		MsalHostingExtensions.DefaultEntraScopes.Should().HaveCount(5);
	}

}
