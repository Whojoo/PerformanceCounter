using System.Diagnostics;
using System.Text;

using Microsoft.Extensions.Logging;

namespace Whojoo.PerformanceCounter;

public sealed class PerformanceCounter(ILogger logger, string performanceCounterName)
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private readonly ILogger _logger = logger;

    private readonly StringBuilder _logBuilder = new StringBuilder()
        .Append("Starting performance counter {PerformanceCounterName}\n");
    private readonly List<object[]> _arguments = [[performanceCounterName]];

    public PerformanceCounter RecordStep(string stepName, Action step)
    {
        var startElapsed = _stopwatch.Elapsed;
        step();
        var endElapsed = _stopwatch.Elapsed;

        var elapsed = endElapsed - startElapsed;

        _logBuilder.Append($"- Step {{{stepName}}}: {{{stepName}ElapsedMilliseconds}} ms\n");

        _arguments.Add([stepName, elapsed.TotalMilliseconds]);

        return this;
    }

    public async Task<PerformanceCounter> RecordStepAsync(string stepName, Func<Task> step)
    {
        var startElapsed = _stopwatch.Elapsed;
        await step();
        var endElapsed = _stopwatch.Elapsed;

        var elapsed = endElapsed - startElapsed;

        _logBuilder.Append($"- Step {{{stepName}}}: {{{stepName}ElapsedMilliseconds}} ms\n");

        _arguments.Add([stepName, elapsed.TotalMilliseconds]);

        return this;
    }

    public void StopAndReport()
    {
        _stopwatch.Stop();

        var totalElapsed = _stopwatch.Elapsed;

        _logBuilder.Append($"- Total elapsed: {totalElapsed.TotalMilliseconds} ms");
        _arguments.Add([totalElapsed.TotalMilliseconds]);

        var messageFormat = _logBuilder.ToString();
        var arguments = _arguments.SelectMany(argumentArray => argumentArray).ToArray();

#pragma warning disable CA2254
        _logger.LogInformation(messageFormat, arguments);
#pragma warning restore CA2254
    }
}
