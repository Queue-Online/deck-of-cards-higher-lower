using test_api.Models;
using test_api.Services;

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
}