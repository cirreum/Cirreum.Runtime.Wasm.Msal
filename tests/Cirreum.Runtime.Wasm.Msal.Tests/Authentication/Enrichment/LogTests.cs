namespace Cirreum.Runtime.Tests.Authentication.Enrichment;

using Cirreum.Runtime.Authentication.Enrichment;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

public class LogTests {

	private readonly ILogger _logger;

	public LogTests() {
		_logger = Substitute.For<ILogger>();
		_logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
	}

	[Fact]
	public void EnrichingProfile_LogsDebugMessage() {
		// Arrange
		var enricherName = "TestEnricher";

		// Act
		_logger.EnrichingProfile(enricherName);

		// Assert
#pragma warning disable CA1873 // Avoid potentially expensive logging
		_logger.Received(1).Log(
			LogLevel.Debug,
			Arg.Any<EventId>(),
			Arg.Any<object>(),
			Arg.Any<Exception?>(),
			Arg.Any<Func<object, Exception?, string>>());
#pragma warning restore CA1873 // Avoid potentially expensive logging
	}

	[Fact]
	public void MissingTenantId_LogsWarningMessage() {
		// Act
		_logger.MissingTenantId();

		// Assert
		_logger.Received(1).Log(
			LogLevel.Warning,
			Arg.Any<EventId>(),
			Arg.Any<object>(),
			Arg.Any<Exception?>(),
			Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public void MissingObjectId_LogsWarningMessage() {
		// Act
		_logger.MissingObjectId();

		// Assert
		_logger.Received(1).Log(
			LogLevel.Warning,
			Arg.Any<EventId>(),
			Arg.Any<object>(),
			Arg.Any<Exception?>(),
			Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public void ODataError_LogsErrorWithException() {
		// Arrange
		var exception = new InvalidOperationException("Test OData error");

		// Act
		_logger.ODataError(exception);

		// Assert
		_logger.Received(1).Log(
			LogLevel.Error,
			Arg.Any<EventId>(),
			Arg.Any<object>(),
			exception,
			Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public void GraphServiceError_LogsErrorWithException() {
		// Arrange
		var exception = new InvalidOperationException("Test Graph service error");

		// Act
		_logger.GraphServiceError(exception);

		// Assert
		_logger.Received(1).Log(
			LogLevel.Error,
			Arg.Any<EventId>(),
			Arg.Any<object>(),
			exception,
			Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public void UnknownGraphError_LogsErrorWithException() {
		// Arrange
		var exception = new InvalidOperationException("Test unknown error");

		// Act
		_logger.UnknownGraphError(exception);

		// Assert
		_logger.Received(1).Log(
			LogLevel.Error,
			Arg.Any<EventId>(),
			Arg.Any<object>(),
			exception,
			Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public void AllLogMethods_AreExtensionMethods() {
		// This test verifies the methods can be called as extension methods
		// If they weren't extension methods, this wouldn't compile
		var logger = Substitute.For<ILogger>();
		logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);

		// These should all compile and run without throwing
		var act = () => {
			logger.EnrichingProfile("test");
			logger.MissingTenantId();
			logger.MissingObjectId();
			logger.ODataError(new Exception());
			logger.GraphServiceError(new Exception());
			logger.UnknownGraphError(new Exception());
		};

		act.Should().NotThrow();
	}

}
