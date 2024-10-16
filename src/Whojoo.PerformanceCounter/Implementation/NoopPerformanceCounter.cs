namespace Whojoo.PerformanceCounter.Implementation;

internal sealed class NoopPerformanceCounter : IPerformanceCounter
{
    public void RecordStep(string stepName, Action step) => step();

    public T RecordStep<T>(string stepName, Func<T> step) => step();

    public async Task RecordStepAsync(string stepName, Func<Task> step) => await step();

    public async Task<T> RecordStepAsync<T>(string stepName, Func<Task<T>> step) => await step();

    public void StopAndReport()
    {
    }

    public void Report()
    {
    }

    public void Restart(string? newPerformanceCounterName = null)
    {
    }
}