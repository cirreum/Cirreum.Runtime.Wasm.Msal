namespace Cirreum.Runtime.Tests.Authentication;

using System.Text.Json;
using Cirreum.Runtime.Authentication;
using FluentAssertions;
using Xunit;

public class EntraUserAccountTests {

	[Fact]
	public void Roles_DefaultsToEmptyList() {
		// Arrange & Act
		var account = new EntraUserAccount();

		// Assert
		account.Roles.Should().NotBeNull();
		account.Roles.Should().BeEmpty();
	}

	[Fact]
	public void Roles_CanBeAssigned() {
		// Arrange
		var account = new EntraUserAccount();
		var roles = new List<string> { "Admin", "User", "Manager" };

		// Act
		account.Roles = roles;

		// Assert
		account.Roles.Should().BeEquivalentTo(roles);
	}

	[Fact]
	public void Roles_SerializesWithCorrectPropertyName() {
		// Arrange
		var account = new EntraUserAccount {
			Roles = ["Admin", "User"]
		};

		// Act
		var json = JsonSerializer.Serialize(account);

		// Assert
		json.Should().Contain("\"roles\":");
		json.Should().Contain("\"Admin\"");
		json.Should().Contain("\"User\"");
	}

	[Fact]
	public void Roles_DeserializesFromJson() {
		// Arrange
		var json = """{"roles":["Reader","Writer"]}""";

		// Act
		var account = JsonSerializer.Deserialize<EntraUserAccount>(json);

		// Assert
		account.Should().NotBeNull();
		account!.Roles.Should().BeEquivalentTo(["Reader", "Writer"]);
	}

	[Fact]
	public void EntraUserAccount_InheritsFromRemoteUserAccount() {
		// Arrange & Act
		var account = new EntraUserAccount();

		// Assert
		account.Should().BeAssignableTo<Microsoft.AspNetCore.Components.WebAssembly.Authentication.RemoteUserAccount>();
	}

}
