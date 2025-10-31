using test_api.Models;
using test_api.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace test_api.Extensions;

public static class DeckOfCardsEndpoints
{
    public static WebApplication MapDeckOfCardsEndpoints(this WebApplication app)
    {
        // Create a new shuffled deck
        app.MapGet("/cards/deck/new", CreateNewDeck)
            .WithName("CreateNewDeck")
            .WithSummary("Create a new shuffled deck")
            .WithDescription("Creates a new shuffled deck of cards")
            .WithOpenApi()
            .Produces<Deck>(200)
            .Produces(400)
            .Produces(500);

        // Draw cards from a deck
        app.MapGet("/cards/deck/{deckId}/draw", DrawCards)
            .WithName("DrawCards")
            .WithSummary("Draw cards from a deck")
            .WithDescription("Draw a specified number of cards from an existing deck")
            .WithOpenApi()
            .Produces<DrawCardsResponse>(200)
            .Produces(400)
            .Produces(404)
            .Produces(500);

        // Draw exactly 5 cards from a deck
        app.MapGet("/cards/deck/{deckId}/draw-five", DrawFiveCards)
            .WithName("DrawFiveCards")
            .WithSummary("Draw five cards from a deck")
            .WithDescription("Draw exactly 5 cards from an existing deck")
            .WithOpenApi()
            .Produces<DrawCardsResponse>(200)
            .Produces(400)
            .Produces(404)
            .Produces(500);

        // Shuffle an existing deck
        app.MapGet("/cards/deck/{deckId}/shuffle", ShuffleDeck)
            .WithName("ShuffleDeck")
            .WithSummary("Shuffle an existing deck")
            .WithDescription("Shuffle the remaining cards in an existing deck")
            .WithOpenApi()
            .Produces<Deck>(200)
            .Produces(400)
            .Produces(404)
            .Produces(500);

        // Get logs
        app.MapGet("/status/logs", GetLogs)
            .WithName("GetLogs")
            .WithSummary("Get application logs")
            .WithDescription("Retrieve recent application logs")
            .WithOpenApi()
            .Produces<string[]>(200)
            .Produces(500);

        // Get health status
        app.MapGet("/status/health", GetHealthStatus)
            .WithName("GetHealthStatus")
            .WithSummary("Get health status")
            .WithDescription("Retrieve current health status of the API")
            .WithOpenApi()
            .Produces(200)
            .Produces(500);

        return app;
    }

    private static async Task<IResult> CreateNewDeck(DeckOfCardsService deckService, int deckCount = 1)
    {
        if (deckCount <= 0 || deckCount > 10)
        {
            return Results.BadRequest(new { error = "Deck count must be between 1 and 10" });
        }

        var deck = await deckService.CreateNewDeckAsync(deckCount);
        return deck is not null 
            ? Results.Ok(deck) 
            : Results.Problem("Failed to create new deck");
    }

    private static async Task<IResult> DrawCards(string deckId, DeckOfCardsService deckService, int count = 2)
    {
        if (string.IsNullOrWhiteSpace(deckId))
        {
            return Results.BadRequest(new { error = "Deck ID cannot be empty" });
        }

        if (count <= 0 || count > 52)
        {
            return Results.BadRequest(new { error = "Count must be between 1 and 52" });
        }

        var result = await deckService.DrawCardsAsync(deckId, count);
        
        if (result == null)
        {
            return Results.NotFound(new { error = $"Deck '{deckId}' not found or error occurred" });
        }

        if (!result.Success)
        {
            return Results.Problem("Failed to draw cards from deck");
        }

        return Results.Ok(result);
    }

    private static async Task<IResult> DrawFiveCards(string deckId, DeckOfCardsService deckService)
    {
        if (string.IsNullOrWhiteSpace(deckId))
        {
            return Results.BadRequest(new { error = "Deck ID cannot be empty" });
        }

        var result = await deckService.DrawFiveCardsAsync(deckId);
        
        if (result == null)
        {
            return Results.NotFound(new { error = $"Deck '{deckId}' not found or error occurred" });
        }

        if (!result.Success)
        {
            return Results.Problem("Failed to draw five cards from deck");
        }

        return Results.Ok(result);
    }

    private static async Task<IResult> ShuffleDeck(string deckId, DeckOfCardsService deckService)
    {
        if (string.IsNullOrWhiteSpace(deckId))
        {
            return Results.BadRequest(new { error = "Deck ID cannot be empty" });
        }

        var deck = await deckService.ShuffleDeckAsync(deckId);
        
        if (deck == null)
        {
            return Results.NotFound(new { error = $"Deck '{deckId}' not found or error occurred" });
        }

        if (!deck.Success)
        {
            return Results.Problem("Failed to shuffle deck");
        }

        return Results.Ok(deck);
    }

    private static async Task<IResult> GetLogs(IWebHostEnvironment env, int limit = 100)
    {
        try
        {
            var logsDirectory = Path.Combine(env.ContentRootPath, "logs");
            if (!Directory.Exists(logsDirectory))
            {
                return Results.Ok(new { logs = Array.Empty<string>(), message = "Logs directory not found" });
            }

            var logFiles = Directory.GetFiles(logsDirectory, "log-*.txt")
                .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                .Take(1)
                .ToList();

            if (logFiles.Count == 0)
            {
                return Results.Ok(new { logs = Array.Empty<string>(), message = "No log files found" });
            }

            var latestLogFile = logFiles[0];
            var logLines = await File.ReadAllLinesAsync(latestLogFile);
            var recentLogs = logLines
                .TakeLast(limit)
                .ToArray();

            return Results.Ok(new { logs = recentLogs, file = Path.GetFileName(latestLogFile) });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Failed to read logs: {ex.Message}");
        }
    }

    private static async Task<IResult> GetHealthStatus(HealthCheckService healthCheckService)
    {
        try
        {
            var healthReport = await healthCheckService.CheckHealthAsync();
            
            var status = new
            {
                status = healthReport.Status.ToString(),
                totalDuration = healthReport.TotalDuration.TotalMilliseconds,
                timestamp = DateTime.UtcNow,
                entries = healthReport.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    duration = e.Value.Duration.TotalMilliseconds,
                    data = e.Value.Data
                })
            };

            return Results.Ok(status);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Failed to get health status: {ex.Message}");
        }
    }
}