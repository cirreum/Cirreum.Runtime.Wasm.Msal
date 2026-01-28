namespace Cirreum.Runtime.Tests.Authentication.Builders;

using Cirreum.Runtime.Authentication.Builders;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class EntraExternalBuilderTests {

	[Fact]
	public void Constructor_WithAuthBuilder_SetsBothProperties() {
		// Arrange
		var services = new ServiceCollection();
		var authBuilder = new EntraAuthenticationBuilder(services);

		// Act
		var externalBuilder = new EntraExternalBuilder(authBuilder);

		// Assert
		externalBuilder.Builder.Should().BeSameAs(authBuilder);
		externalBuilder.Services.Should().BeSameAs(services);
	}

	[Fact]
	public void Services_ReturnsServiceCollectionFromUnderlyingBuilder() {
		// Arrange
		var services = new ServiceCollection();
		var authBuilder = new EntraAuthenticationBuilder(services);
		var externalBuilder = new EntraExternalBuilder(authBuilder);

		// Act
		var result = externalBuilder.Services;

		// Assert
		result.Should().BeSameAs(services);
	}

	[Fact]
	public void Builder_ReturnsUnderlyingAuthBuilder() {
		// Arrange
		var services = new ServiceCollection();
		var authBuilder = new EntraAuthenticationBuilder(services);
		var externalBuilder = new EntraExternalBuilder(authBuilder);

		// Act
		var result = externalBuilder.Builder;

		// Assert
		result.Should().BeSameAs(authBuilder);
	}

	[Fact]
	public void EntraExternalBuilder_ImplementsIEntraExternalBuilder() {
		// Arrange
		var services = new ServiceCollection();
		var authBuilder = new EntraAuthenticationBuilder(services);

		// Act
		var externalBuilder = new EntraExternalBuilder(authBuilder);

		// Assert
		externalBuilder.Should().BeAssignableTo<IEntraExternalBuilder>();
	}

}
