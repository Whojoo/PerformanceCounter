using Whojoo.PerformanceCounter;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.MapGet("/test", async (ILogger<Program> logger) =>
{
    var performanceCounter = PerformanceCounterFactory.Start(logger);

    performanceCounter.RecordStep("SyncRecordNoResult", () =>
    {
        foreach (var step in Enumerable.Range(1, 1_000_000))
        {
            var foo = int.MaxValue - step;
        }
    });

    var recordSyncResult = performanceCounter.RecordStep("SyncRecordWithResult", () =>
    {
        var returnResult = int.MaxValue;
        foreach (var step in Enumerable.Range(1, 1_000_000))
        {
            returnResult -= step;
        }

        return returnResult;
    });

    await performanceCounter.RecordStepAsync("AsyncRecordNoResult", async () =>
    {
        await Task.Delay(TimeSpan.FromMilliseconds(500));
    });

    var recordAsyncResult = await performanceCounter.RecordStepAsync("AsyncRecordWithResult", async () =>
    {
        await Task.Delay(TimeSpan.FromMilliseconds(500));
        return 500;
    });

    performanceCounter.StopAndReport();

    return recordAsyncResult + recordSyncResult;
});
app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
