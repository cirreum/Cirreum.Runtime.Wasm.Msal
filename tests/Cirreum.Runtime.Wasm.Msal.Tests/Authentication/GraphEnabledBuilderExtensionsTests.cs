namespace Cirreum.Runtime.Tests.Authentication;

using Cirreum.Runtime.Authentication;
using FluentAssertions;
using Xunit;

public class GraphEnabledBuilderExtensionsTests {

	[Fact]
	public void MinimalGraphScopes_ContainsUserRead() {
		// Arrange & Act
		var scopes = GraphEnabledBuilderExtensions.MinimalGraphScopes;

		// Assert
		scopes.Should().Contain("User.Read");
	}

	[Fact]
	public void MinimalGraphScopes_HasSingleScope() {
		// Arrange & Act
		var scopes = GraphEnabledBuilderExtensions.MinimalGraphScopes;

		// Assert
		scopes.Should().HaveCount(1);
	}

	[Fact]
	public void ExtendedGraphScopes_ContainsExpectedScopes() {
		// Arrange
		var expectedScopes = new List<string> {
			"User.Read",
			"MailboxSettings.Read",
			"Directory.AccessAsUser.All"
		};

		// Act
		var scopes = GraphEnabledBuilderExtensions.ExtendedGraphScopes;

		// Assert
		scopes.Should().BeEquivalentTo(expectedScopes);
	}

	[Fact]
	public void ExtendedGraphScopes_HasThreeScopes() {
		// Arrange & Act
		var scopes = GraphEnabledBuilderExtensions.ExtendedGraphScopes;

		// Assert
		scopes.Should().HaveCount(3);
	}

	[Fact]
	public void ExtendedGraphScopes_ContainsUserRead() {
		GraphEnabledBuilderExtensions.ExtendedGraphScopes.Should().Contain("User.Read");
	}

	[Fact]
	public void ExtendedGraphScopes_ContainsMailboxSettingsRead() {
		GraphEnabledBuilderExtensions.ExtendedGraphScopes.Should().Contain("MailboxSettings.Read");
	}

	[Fact]
	public void ExtendedGraphScopes_ContainsDirectoryAccessAsUserAll() {
		GraphEnabledBuilderExtensions.ExtendedGraphScopes.Should().Contain("Directory.AccessAsUser.All");
	}

}
