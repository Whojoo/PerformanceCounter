using FluentAssertions;

using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace Whojoo.PerformanceCounter.Tests;

public class PerformanceCounterFactoryTests(ITestOutputHelper testOutputHelper)
{
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;

    private readonly CustomTestLogger _logger =
        CustomTestLogger.CreateLogger<PerformanceCounterFactoryTests>(testOutputHelper);

    [Fact]
    public void Start_WhenDefaultOptionsAreUsed_ShouldCreatePerformanceLogger()
    {
        // Arrange
        // Act
        var performanceCounter = PerformanceCounterFactory.Start(_logger);

        // Assert
        performanceCounter
            .Should()
            .BeOfType<Implementation.PerformanceCounter>();
    }

    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void Start_WhenLogLevelIsValid_ShouldCreatePerformanceCounter(LogLevel logLevel)
    {
        // Arrange
        var options = new PerformanceCounterOptions { LogLevel = logLevel };

        // Act
        var performanceCounter = PerformanceCounterFactory.Start(_logger, options);

        // Assert
        performanceCounter
            .Should()
            .BeOfType<Implementation.PerformanceCounter>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(10)]
    public void Start_WhenLogLevelIsInvalid_ShouldThrowArgumentException(int logLevel)
    {
        // Arrange
        var options = new PerformanceCounterOptions { LogLevel = (LogLevel)logLevel };

        // Act
        var act = () => PerformanceCounterFactory.Start(_logger, options);

        // Assert
        act
            .Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void Start_WhenLogLevelIsNone_ShouldCreateNoopPerformanceCounter()
    {
        // Arrange
        var options = new PerformanceCounterOptions { LogLevel = LogLevel.None };

        // Act
        var performanceCounter = PerformanceCounterFactory.Start(_logger, options);

        // Assert
        performanceCounter
            .Should()
            .BeOfType<Implementation.NoopPerformanceCounter>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("          ")]
    [InlineData(null)]
    public void Start_WhenPerformanceCounterNameIsInvalid_ShouldThrowArgumentException(string? performanceCounterName)
    {
        // Arrange
#pragma warning disable CS8601 // Possible null reference assignment.
        var options = new PerformanceCounterOptions { PerformanceCounterName = performanceCounterName };
#pragma warning restore CS8601 // Possible null reference assignment.

        // Act
        var act = () => PerformanceCounterFactory.Start(_logger, options);

        // Assert
        act
            .Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void Start_WhenIsEnabledIsFalse_ShouldCreateNoopPerformanceCounter()
    {
        // Arrange
        var options = new PerformanceCounterOptions { IsEnabled = false };

        // Act
        var performanceCounter = PerformanceCounterFactory.Start(_logger, options);

        // Assert
        performanceCounter
            .Should()
            .BeOfType<Implementation.NoopPerformanceCounter>();
    }

    [Fact]
    public void Start_WhenLogLevelIsEnabled_ShouldCreatePerformanceCounter()
    {
        // Arrange
        var logger = CustomTestLogger
            .CreateLogger<PerformanceCounterFactoryTests>(_testOutputHelper, LogLevel.Information);

        // Act
        var performanceCounter = PerformanceCounterFactory.Start(logger);

        // Assert
        performanceCounter
            .Should()
            .BeOfType<Implementation.PerformanceCounter>();
    }

    [Fact]
    public void Start_WhenLogLevelIsDisabled_ShouldCreateNoopPerformanceCounter()
    {
        // Arrange
        var logger = CustomTestLogger.CreateLogger<PerformanceCounterFactoryTests>(_testOutputHelper, LogLevel.Warning);

        // Act
        var performanceCounter = PerformanceCounterFactory.Start(logger);

        // Assert
        performanceCounter
            .Should()
            .BeOfType<Implementation.NoopPerformanceCounter>();
    }
}