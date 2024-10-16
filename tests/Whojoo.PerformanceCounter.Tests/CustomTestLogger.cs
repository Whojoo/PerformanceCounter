using System.Text;

using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace Whojoo.PerformanceCounter.Tests;

internal class CustomTestLogger(
    ITestOutputHelper testOutputHelper,
    LoggerExternalScopeProvider scopeProvider,
    string categoryName)
    : ILogger
{
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;
    private readonly LoggerExternalScopeProvider _scopeProvider = scopeProvider;
    private readonly string _categoryName = categoryName;
    private readonly StringBuilder _producedLogsBuilder = new();

    public string ProducedLogs => _producedLogsBuilder.ToString();

    public static CustomTestLogger CreateLogger(ITestOutputHelper testOutputHelper) =>
        new(testOutputHelper, new LoggerExternalScopeProvider(), string.Empty);

    public static CustomTestLogger CreateLogger<T>(ITestOutputHelper testOutputHelper) =>
        new CustomTestLogger<T>(testOutputHelper, new LoggerExternalScopeProvider());

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var logBuilder = new StringBuilder()
            .Append(GetLogLevelString(logLevel))
            .Append(" [")
            .Append(_categoryName)
            .Append("] ")
            .Append(formatter(state, exception));

        if (exception is not null)
        {
            logBuilder
                .Append('\n')
                .Append(exception);
        }

        _scopeProvider.ForEachScope(
            (scope, stateLogBuilder) =>
            {
                stateLogBuilder.Append("\n => ");
                stateLogBuilder.Append(scope);
            },
            logBuilder);

        _testOutputHelper.WriteLine(logBuilder.ToString());
    }

    public bool IsEnabled(LogLevel logLevel) => logLevel is not LogLevel.None;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => _scopeProvider.Push(state);

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }
}

internal class CustomTestLogger<T>(ITestOutputHelper testOutputHelper, LoggerExternalScopeProvider scopeProvider)
    : CustomTestLogger(testOutputHelper, scopeProvider, typeof(T).FullName!), ILogger<T>
{
}