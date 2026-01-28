namespace Cirreum.Runtime.Tests.Authentication.Providers;

using Cirreum.Runtime.Authentication.Providers;
using FluentAssertions;
using Xunit;

public class DefaultGraphServiceClientProviderTests {

	[Fact]
	public void HttpClientName_HasExpectedValue() {
		// Act
		var name = DefaultGraphServiceClientProvider.HttpClientName;

		// Assert
		name.Should().Be("_GraphHttpClient");
	}

	[Fact]
	public void GraphUrl_HasExpectedValue() {
		// Act
		var url = DefaultGraphServiceClientProvider.GraphUrl;

		// Assert
		url.Should().Be("https://graph.microsoft.com/v1.0");
	}

	[Fact]
	public void GraphUrl_IsHttps() {
		// Act
		var url = DefaultGraphServiceClientProvider.GraphUrl;

		// Assert
		url.Should().StartWith("https://");
	}

	[Fact]
	public void GraphUrl_PointsToMicrosoftGraph() {
		// Act
		var url = DefaultGraphServiceClientProvider.GraphUrl;

		// Assert
		url.Should().Contain("graph.microsoft.com");
	}

	[Fact]
	public void GraphUrl_UsesV1Endpoint() {
		// Act
		var url = DefaultGraphServiceClientProvider.GraphUrl;

		// Assert
		url.Should().EndWith("/v1.0");
	}

}
