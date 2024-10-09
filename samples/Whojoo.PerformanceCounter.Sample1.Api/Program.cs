using Whojoo.PerformanceCounter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/test", async (ILogger<Program> logger) =>
{
    var performanceCounter = PerformanceCounterFactory.StartDefault(logger, "Test");

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

app.Run();

