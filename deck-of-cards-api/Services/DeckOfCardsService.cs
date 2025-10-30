using test_api.Models;
using System.Text.Json;

namespace test_api.Services;

public class DeckOfCardsService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<DeckOfCardsService> _logger;

    public DeckOfCardsService(HttpClient httpClient, ILogger<DeckOfCardsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
    }

    public async Task<Deck?> CreateNewDeckAsync(int deckCount = 1)
    {
        try
        {
            _logger.LogInformation("Creating new deck with count: {DeckCount}", deckCount);

            var response = await _httpClient.GetAsync($"deck/new/shuffle/?deck_count={deckCount}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to create deck, Status: {StatusCode}", response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<DeckApiResponse>(json, _jsonOptions);

            if (apiResponse == null || !apiResponse.Success)
            {
                _logger.LogWarning("Invalid response when creating deck");
                return null;
            }

            return new Deck(
                Success: apiResponse.Success,
                DeckId: apiResponse.Deck_Id,
                Shuffled: apiResponse.Shuffled,
                Remaining: apiResponse.Remaining
            );
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error when creating deck");
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timeout when creating deck");
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error when creating deck");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when creating deck");
            return null;
        }
    }

    public async Task<DrawCardsResponse?> DrawCardsAsync(string deckId, int count = 2)
    {
        try
        {
            _logger.LogInformation("Drawing {Count} cards from deck: {DeckId}", count, deckId);

            var response = await _httpClient.GetAsync($"deck/{deckId}/draw/?count={count}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to draw cards from deck {DeckId}, Status: {StatusCode}", 
                    deckId, response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<DrawCardsApiResponse>(json, _jsonOptions);

            if (apiResponse == null || !apiResponse.Success)
            {
                _logger.LogWarning("Invalid response when drawing cards from deck {DeckId}", deckId);
                return null;
            }

            return new DrawCardsResponse(
                Success: apiResponse.Success,
                DeckId: apiResponse.Deck_Id,
                Cards: apiResponse.Cards.Select(MapToCard).ToArray(),
                Remaining: apiResponse.Remaining
            );
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error when drawing cards from deck {DeckId}", deckId);
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timeout when drawing cards from deck {DeckId}", deckId);
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error when drawing cards from deck {DeckId}", deckId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when drawing cards from deck {DeckId}", deckId);
            return null;
        }
    }

    public async Task<DrawCardsResponse?> DrawFiveCardsAsync(string deckId)
    {
        return await DrawCardsAsync(deckId, 5);
    }

    public async Task<Deck?> ShuffleDeckAsync(string deckId)
    {
        try
        {
            _logger.LogInformation("Shuffling deck: {DeckId}", deckId);

            var response = await _httpClient.GetAsync($"deck/{deckId}/shuffle/");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to shuffle deck {DeckId}, Status: {StatusCode}", 
                    deckId, response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<DeckApiResponse>(json, _jsonOptions);

            if (apiResponse == null || !apiResponse.Success)
            {
                _logger.LogWarning("Invalid response when shuffling deck {DeckId}", deckId);
                return null;
            }

            return new Deck(
                Success: apiResponse.Success,
                DeckId: apiResponse.Deck_Id,
                Shuffled: apiResponse.Shuffled,
                Remaining: apiResponse.Remaining
            );
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error when shuffling deck {DeckId}", deckId);
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timeout when shuffling deck {DeckId}", deckId);
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error when shuffling deck {DeckId}", deckId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when shuffling deck {DeckId}", deckId);
            return null;
        }
    }

    private static Card MapToCard(CardApiResponse cardApi)
    {
        return new Card(
            Code: cardApi.Code,
            Image: cardApi.Image,
            Images: new CardImages(cardApi.Images.Svg, cardApi.Images.Png),
            Value: cardApi.Value,
            Suit: cardApi.Suit
        );
    }
}