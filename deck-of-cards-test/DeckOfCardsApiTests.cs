using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using test_api.Models;
using test_api.Services;

namespace deck_of_cards_test;

public class DeckOfCardsApiTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public DeckOfCardsApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateNewDeck_ValidRequest_ReturnsOkWithDeck()
    {
        // Act
        var response = await _client.GetAsync("/cards/deck/new?deckCount=1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var deck = await response.Content.ReadFromJsonAsync<Deck>();
        Assert.NotNull(deck);
        Assert.True(deck.Success);
        Assert.NotNull(deck.DeckId);
        Assert.Equal(52, deck.Remaining);
    }

    [Fact]
    public async Task DrawCards_ValidDeckId_ReturnsOkWithCards()
    {
        // Arrange - Create a deck first
        var createResponse = await _client.GetAsync("/cards/deck/new?deckCount=1");
        var deck = await createResponse.Content.ReadFromJsonAsync<Deck>();
        Assert.NotNull(deck);

        // Act - Draw cards
        var drawResponse = await _client.GetAsync($"/cards/deck/{deck.DeckId}/draw?count=2");

        // Assert
        Assert.Equal(HttpStatusCode.OK, drawResponse.StatusCode);
        var drawResult = await drawResponse.Content.ReadFromJsonAsync<DrawCardsResponse>();
        Assert.NotNull(drawResult);
        Assert.True(drawResult.Success);
        Assert.Equal(2, drawResult.Cards.Length);
        Assert.Equal(50, drawResult.Remaining);
    }

    [Fact]
    public async Task DrawCards_InvalidDeckId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/cards/deck/invalid-deck-id-12345/draw?count=2");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var errorContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("not found", errorContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DrawFiveCards_ValidDeckId_ReturnsFiveCards()
    {
        // Arrange - Create a deck first
        var createResponse = await _client.GetAsync("/cards/deck/new?deckCount=1");
        var deck = await createResponse.Content.ReadFromJsonAsync<Deck>();
        Assert.NotNull(deck);

        // Act - Draw five cards
        var drawResponse = await _client.GetAsync($"/cards/deck/{deck.DeckId}/draw-five");

        // Assert
        Assert.Equal(HttpStatusCode.OK, drawResponse.StatusCode);
        var drawResult = await drawResponse.Content.ReadFromJsonAsync<DrawCardsResponse>();
        Assert.NotNull(drawResult);
        Assert.True(drawResult.Success);
        Assert.Equal(5, drawResult.Cards.Length);
        Assert.Equal(47, drawResult.Remaining);
    }

    [Fact]
    public async Task ShuffleDeck_ValidDeckId_ReturnsOk()
    {
        // Arrange - Create a deck first
        var createResponse = await _client.GetAsync("/cards/deck/new?deckCount=1");
        var deck = await createResponse.Content.ReadFromJsonAsync<Deck>();
        Assert.NotNull(deck);

        // Act - Shuffle the deck
        var shuffleResponse = await _client.GetAsync($"/cards/deck/{deck.DeckId}/shuffle");

        // Assert
        Assert.Equal(HttpStatusCode.OK, shuffleResponse.StatusCode);
        var shuffledDeck = await shuffleResponse.Content.ReadFromJsonAsync<Deck>();
        Assert.NotNull(shuffledDeck);
        Assert.True(shuffledDeck.Success);
        Assert.True(shuffledDeck.Shuffled);
        Assert.Equal(deck.DeckId, shuffledDeck.DeckId);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}

// Custom WebApplicationFactory that mocks the external API
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing HttpClient registration for DeckOfCardsService
            var httpClientDescriptors = services
                .Where(d => d.ServiceType == typeof(DeckOfCardsService))
                .ToList();
            
            foreach (var descriptor in httpClientDescriptors)
            {
                services.Remove(descriptor);
            }

            // Add a mocked HttpClientFactory with our custom handler
            services.AddHttpClient<DeckOfCardsService>(client =>
            {
                client.BaseAddress = new Uri("https://deckofcardsapi.com/api/");
                client.DefaultRequestHeaders.Add("User-Agent", "DeckOfCardsAPI/1.0");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new MockDeckApiMessageHandler());
        });
    }
}

// Mock message handler that simulates the external Deck of Cards API
public class MockDeckApiMessageHandler : HttpMessageHandler
{
    private static readonly Dictionary<string, DeckApiResponse> _decks = new();
    private static readonly Dictionary<string, List<CardApiResponse>> _deckCards = new();
    private static int _deckCounter = 1;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var uri = request.RequestUri?.ToString() ?? "";
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        // Create new deck
        if (uri.Contains("deck/new/shuffle"))
        {
            var deckCount = ExtractDeckCount(uri);
            var deckId = $"mock-deck-{_deckCounter++}";
            var remaining = 52 * deckCount;

            var deck = new DeckApiResponse(
                Success: true,
                Deck_Id: deckId,
                Shuffled: true,
                Remaining: remaining
            );

            _decks[deckId] = deck;
            _deckCards[deckId] = GenerateDeck(deckCount).ToList();

            var json = JsonSerializer.Serialize(deck, jsonOptions);
            return Task.FromResult(CreateResponse(HttpStatusCode.OK, json));
        }

        // Draw cards
        if (uri.Contains("/draw/"))
        {
            var deckId = ExtractDeckId(uri, "draw");
            var count = ExtractCount(uri);

            if (!_decks.TryGetValue(deckId, out var deck) || !_deckCards.TryGetValue(deckId, out var deckCards))
            {
                return Task.FromResult(CreateResponse(HttpStatusCode.NotFound, "{\"success\":false}"));
            }

            var cards = deckCards.Take(count).ToArray();
            deckCards = deckCards.Skip(count).ToList();
            _deckCards[deckId] = deckCards;
            _decks[deckId] = deck with { Remaining = deckCards.Count };

            var drawResponse = new DrawCardsApiResponse(
                Success: true,
                Deck_Id: deckId,
                Remaining: deckCards.Count,
                Cards: cards
            );

            var drawJson = JsonSerializer.Serialize(drawResponse, jsonOptions);
            return Task.FromResult(CreateResponse(HttpStatusCode.OK, drawJson));
        }

        // Shuffle deck
        if (uri.Contains("/shuffle/"))
        {
            var deckId = ExtractDeckId(uri, "shuffle");

            if (!_decks.TryGetValue(deckId, out var deck))
            {
                return Task.FromResult(CreateResponse(HttpStatusCode.NotFound, "{\"success\":false}"));
            }

            deck = deck with { Shuffled = true };
            _decks[deckId] = deck;

            var shuffleJson = JsonSerializer.Serialize(deck, jsonOptions);
            return Task.FromResult(CreateResponse(HttpStatusCode.OK, shuffleJson));
        }

        return Task.FromResult(CreateResponse(HttpStatusCode.NotFound, "{\"success\":false}"));
    }

    private static HttpResponseMessage CreateResponse(HttpStatusCode statusCode, string content)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };
    }

    private static string ExtractDeckId(string uri, string endpoint)
    {
        var parts = uri.Split('/');
        var drawIndex = Array.IndexOf(parts, endpoint.Split('/').First());
        return drawIndex > 0 ? parts[drawIndex - 1] : "";
    }

    private static int ExtractCount(string uri)
    {
        var match = System.Text.RegularExpressions.Regex.Match(uri, @"count=(\d+)");
        return match.Success ? int.Parse(match.Groups[1].Value) : 2;
    }

    private static int ExtractDeckCount(string uri)
    {
        var match = System.Text.RegularExpressions.Regex.Match(uri, @"deck_count=(\d+)");
        return match.Success ? int.Parse(match.Groups[1].Value) : 1;
    }

    private static IEnumerable<CardApiResponse> GenerateDeck(int deckCount)
    {
        var suits = new[] { "SPADES", "HEARTS", "DIAMONDS", "CLUBS" };
        var values = new[] { "ACE", "2", "3", "4", "5", "6", "7", "8", "9", "10", "JACK", "QUEEN", "KING" };
        var valueCodes = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        var suitCodes = new[] { "S", "H", "D", "C" };

        var deckList = new List<CardApiResponse>();

        for (int d = 0; d < deckCount; d++)
        {
            for (int s = 0; s < suits.Length; s++)
            {
                for (int v = 0; v < values.Length; v++)
                {
                    var code = $"{valueCodes[v]}{suitCodes[s]}";
                    deckList.Add(new CardApiResponse(
                        Code: code,
                        Value: values[v],
                        Suit: suits[s],
                        Image: $"https://deckofcardsapi.com/static/img/{code}.png",
                        Images: new CardImagesApiResponse(
                            Svg: $"https://deckofcardsapi.com/static/img/{code}.svg",
                            Png: $"https://deckofcardsapi.com/static/img/{code}.png"
                        )
                    ));
                }
            }
        }

        // Shuffle the deck
        var random = new Random();
        return deckList.OrderBy(_ => random.Next());
    }
}

