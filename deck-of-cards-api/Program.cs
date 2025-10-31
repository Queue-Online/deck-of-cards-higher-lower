using test_api.Services;
using test_api.Extensions;
using Serilog;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Check if observability is enabled via feature flag
var isObservabilityEnabled = builder.Configuration.GetValue<bool>("FeatureToggles:Observability", false);

// Configure Serilog if observability is enabled
if (isObservabilityEnabled)
{
    // Use bootstrap logger pattern for early startup logging
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateBootstrapLogger();

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext(),
        preserveStaticLogger: true,
        writeToProviders: false); // Set to false to avoid duplicate logs with standard .NET logging
}

// Add services to the container.
builder.Services.AddOpenApi();

// Add Feature Toggle Service
builder.Services.AddSingleton<FeatureToggleService>();

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<ApiHealthCheck>("api_health", tags: new[] { "ready", "live" });

// Add HttpClient and DeckOfCardsService with proper configuration
builder.Services.AddHttpClient<DeckOfCardsService>(client =>
{
    var deckApiConfig = builder.Configuration.GetSection("DeckOfCardsApi");
    var baseUrl = deckApiConfig.GetValue<string>("BaseUrl") ?? "https://deckofcardsapi.com/api/";
    var timeoutSeconds = deckApiConfig.GetValue<int>("TimeoutSeconds");
    
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("User-Agent", "DeckOfCardsAPI/1.0");
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds > 0 ? timeoutSeconds : 30);
});

// Add CORS support
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

// Add global exception handling
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("""{"error": "An internal server error occurred"}""");
    });
});

app.UseHttpsRedirection();

// Map health check endpoints (always available)
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});

// Add request logging if observability is enabled
if (isObservabilityEnabled)
{
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
        };
    });
}

// Map Deck of Cards endpoints
app.MapDeckOfCardsEndpoints();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("Starting Deck of Cards API - Observability: {Status}", isObservabilityEnabled ? "enabled" : "disabled");
    
    await app.RunAsync();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    if (isObservabilityEnabled)
    {
        Log.CloseAndFlush();
    }
}

// Make Program class accessible for integration tests
public partial class Program { }