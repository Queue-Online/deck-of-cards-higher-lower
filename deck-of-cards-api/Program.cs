using test_api.Services;
using test_api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

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

// Map Deck of Cards endpoints
app.MapDeckOfCardsEndpoints();

await app.RunAsync();

// Make Program class accessible for integration tests
public partial class Program { }