namespace Cirreum.Runtime.Tests.Authentication.Builders;

using Cirreum.Runtime.Authentication.Builders;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class EntraAuthenticationBuilderTests {

	[Fact]
	public void Constructor_WithServiceCollection_SetsServicesProperty() {
		// Arrange
		var services = new ServiceCollection();

		// Act
		var builder = new EntraAuthenticationBuilder(services);

		// Assert
		builder.Services.Should().BeSameAs(services);
	}

	[Fact]
	public void Services_ReturnsProvidedServiceCollection() {
		// Arrange
		var services = new ServiceCollection();
		var builder = new EntraAuthenticationBuilder(services);

		// Act
		var result = builder.Services;

		// Assert
		result.Should().NotBeNull();
		result.Should().BeSameAs(services);
	}

	[Fact]
	public void EntraAuthenticationBuilder_ImplementsIEntraAuthenticationBuilder() {
		// Arrange
		var services = new ServiceCollection();

		// Act
		var builder = new EntraAuthenticationBuilder(services);

		// Assert
		builder.Should().BeAssignableTo<IEntraAuthenticationBuilder>();
	}

}
