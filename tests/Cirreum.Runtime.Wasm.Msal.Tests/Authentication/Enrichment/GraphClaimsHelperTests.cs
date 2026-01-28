namespace Cirreum.Runtime.Tests.Authentication.Enrichment;

using System.Security.Claims;
using Cirreum.Runtime.Authentication.Enrichment;
using FluentAssertions;
using Xunit;

public class GraphClaimsHelperTests {

	[Fact]
	public void GetTenantId_WithTidClaim_ReturnsTenantId() {
		// Arrange
		var identity = new ClaimsIdentity([
			new Claim("tid", "test-tenant-id")
		]);

		// Act
		var result = identity.GetTenantId();

		// Assert
		result.Should().Be("test-tenant-id");
	}

	[Fact]
	public void GetTenantId_WithoutTidClaim_ReturnsNull() {
		// Arrange
		var identity = new ClaimsIdentity();

		// Act
		var result = identity.GetTenantId();

		// Assert
		result.Should().BeNull();
	}

	[Fact]
	public void GetObjectId_WithOidClaim_ReturnsObjectId() {
		// Arrange
		var identity = new ClaimsIdentity([
			new Claim("oid", "test-object-id")
		]);

		// Act
		var result = identity.GetObjectId();

		// Assert
		result.Should().Be("test-object-id");
	}

	[Fact]
	public void GetObjectId_WithoutOidClaim_ReturnsNull() {
		// Arrange
		var identity = new ClaimsIdentity();

		// Act
		var result = identity.GetObjectId();

		// Assert
		result.Should().BeNull();
	}

	[Theory]
	[InlineData("true", true)]
	[InlineData("false", false)]
	[InlineData("True", true)]
	[InlineData("False", false)]
	public void IsEmailVerified_WithEmailAndXmsEdov_ReturnsParsedValue(string xmsEdovValue, bool expected) {
		// Arrange
		var identity = new ClaimsIdentity([
			new Claim("email", "user@example.com"),
			new Claim("xms_edov", xmsEdovValue)
		]);

		// Act
		var result = identity.IsEmailVerified();

		// Assert
		result.Should().Be(expected);
	}

	[Fact]
	public void IsEmailVerified_WithoutEmailClaim_ReturnsNull() {
		// Arrange
		var identity = new ClaimsIdentity([
			new Claim("xms_edov", "true")
		]);

		// Act
		var result = identity.IsEmailVerified();

		// Assert
		result.Should().BeNull();
	}

	[Fact]
	public void IsEmailVerified_WithoutXmsEdovClaim_ReturnsNull() {
		// Arrange
		var identity = new ClaimsIdentity([
			new Claim("email", "user@example.com")
		]);

		// Act
		var result = identity.IsEmailVerified();

		// Assert
		result.Should().BeNull();
	}

	[Fact]
	public void IsEmailVerified_WithInvalidXmsEdov_ReturnsNull() {
		// Arrange
		var identity = new ClaimsIdentity([
			new Claim("email", "user@example.com"),
			new Claim("xms_edov", "not-a-boolean")
		]);

		// Act
		var result = identity.IsEmailVerified();

		// Assert
		result.Should().BeNull();
	}

	[Theory]
	[InlineData("true", true)]
	[InlineData("false", false)]
	public void IsPhoneNumberVerified_WithValidClaim_ReturnsParsedValue(string value, bool expected) {
		// Arrange
		var identity = new ClaimsIdentity([
			new Claim("phonenumber_verified", value)
		]);

		// Act
		var result = identity.IsPhoneNumberVerified();

		// Assert
		result.Should().Be(expected);
	}

	[Fact]
	public void IsPhoneNumberVerified_WithoutClaim_ReturnsNull() {
		// Arrange
		var identity = new ClaimsIdentity();

		// Act
		var result = identity.IsPhoneNumberVerified();

		// Assert
		result.Should().BeNull();
	}

	[Fact]
	public void IsPhoneNumberVerified_WithInvalidValue_ReturnsNull() {
		// Arrange
		var identity = new ClaimsIdentity([
			new Claim("phonenumber_verified", "invalid")
		]);

		// Act
		var result = identity.IsPhoneNumberVerified();

		// Assert
		result.Should().BeNull();
	}

	[Fact]
	public void UpdatedAt_WithValidDateClaim_ReturnsParsedDate() {
		// Arrange
		var expectedDate = new DateTimeOffset(2024, 6, 15, 10, 30, 0, TimeSpan.Zero);
		var identity = new ClaimsIdentity([
			new Claim("updated_at", expectedDate.ToString("O"))
		]);

		// Act
		var result = identity.UpdatedAt();

		// Assert
		result.Should().Be(expectedDate);
	}

	[Fact]
	public void UpdatedAt_WithoutClaim_ReturnsMinValue() {
		// Arrange
		var identity = new ClaimsIdentity();

		// Act
		var result = identity.UpdatedAt();

		// Assert
		result.Should().Be(DateTimeOffset.MinValue);
	}

	[Fact]
	public void UpdatedAt_WithInvalidDate_ReturnsMinValue() {
		// Arrange
		var identity = new ClaimsIdentity([
			new Claim("updated_at", "not-a-date")
		]);

		// Act
		var result = identity.UpdatedAt();

		// Assert
		result.Should().Be(DateTimeOffset.MinValue);
	}

}
