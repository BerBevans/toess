using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Toess.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeScopes = true;
    options.IncludeFormattedMessage = true;

    options
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .ConfigureOtelService()
        )
        .AddOtlpExporter();
});
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .ConfigureOtelService())
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter()
    )
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter()
    );

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        corsbuilder => corsbuilder.WithOrigins("*")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (ILogger<Program> logger) => // (Tracer tracer) =>
    {
        // using var span = tracer.StartActiveSpan("app.manual-span");
        // span.SetAttribute("app.manual-span.message", "Adding custom spans is also super easy!");

        logger.LogInformation("weatherforecast page visited at {DT}",
            DateTime.UtcNow.ToLongTimeString());

        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 56),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();


app.MapGet("/test", (ILogger<Program> logger) => // (Tracer tracer) =>
    {
        logger.LogInformation("test page visited at {DT}",
            DateTime.UtcNow.ToLongTimeString());

        return "hello world";
    })
    .WithName("TestPage")
    .WithOpenApi();

app.UseCors("AllowAllOrigins");

app.Run();

public partial class Program;

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}