using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

const string serviceName = "WebApi";

var honeycombOptions = builder.Configuration.GetHoneycombOptions();

builder.Logging.AddOpenTelemetry(options =>
{
    options
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName))
        .AddConsoleExporter();
});
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithTracing(tracing => tracing
        .AddHoneycomb(honeycombOptions)
        .AddCommonInstrumentations()
        // .AddAspNetCoreInstrumentation()
        // .AddHttpClientInstrumentation()
        
        // .AddOtlpExporter(option =>
        // {
        //     option.Endpoint = new Uri("https://api.eu1.honeycomb.io/v1/traces");
        //     option.Headers = $"x-honeycomb-team=zGIc4yXnFfasp7M5twLvED";
        //     option.Protocol = OtlpExportProtocol.HttpProtobuf;
        // })
        .AddConsoleExporter())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter()
    );
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(honeycombOptions.ServiceName));


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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (Tracer tracer) =>
{
    using var span = tracer.StartActiveSpan("app.manual-span");
    span.SetAttribute("app.manual-span.message", "Adding custom spans is also super easy!");
    
    var forecast =  Enumerable.Range(1, 5).Select(index =>
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

app.Run();

public partial class Program
{
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
