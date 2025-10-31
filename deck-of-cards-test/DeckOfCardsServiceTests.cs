using test_api.Models;
using test_api.Services;
using System.Net;
using System.Text;
using System.Text.Json;
using Moq;
using Microsoft.Extensions.Logging;

namespace deck_of_cards_test;

public class DeckOfCardsServiceTests : IDisposable
{
    private readonly Mock<ILogger<DeckOfCardsService>> _loggerMock;
    private readonly TestHttpMessageHandler _messageHandler;
    private readonly HttpClient _httpClient;
    private readonly DeckOfCardsService _service;

    public DeckOfCardsServiceTests()
    {
        _loggerMock = new Mock<ILogger<DeckOfCardsService>>();
        _messageHandler = new TestHttpMessageHandler();
        _httpClient = new HttpClient(_messageHandler)
        {
            BaseAddress = new Uri("https://deckofcardsapi.com/api/")
        };
        _service = new DeckOfCardsService(_httpClient, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateNewDeckAsync_Success_ReturnsDeck()
    {
        // Arrange
        var expectedResponse = new DeckApiResponse(
            Success: true,
            Deck_Id: "test-deck-123",
            Shuffled: true,
            Remaining: 52
        );

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        var jsonResponse = JsonSerializer.Serialize(expectedResponse, jsonOptions);

        _messageHandler.SetResponse("deck/new/shuffle/?deck_count=1", HttpStatusCode.OK, jsonResponse);

        // Act
        var result = await _service.CreateNewDeckAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("test-deck-123", result.DeckId);
        Assert.True(result.Shuffled);
        Assert.Equal(52, result.Remaining);
    }

    [Fact]
    public async Task DrawCardsAsync_Success_ReturnsCards()
    {
        // Arrange
        var deckId = "test-deck-123";
        var expectedResponse = new DrawCardsApiResponse(
            Success: true,
            Deck_Id: deckId,
            Remaining: 50,
            Cards: new CardApiResponse[]
            {
                new CardApiResponse(
                    Code: "AS",
                    Value: "ACE",
                    Suit: "SPADES",
                    Image: "https://deckofcardsapi.com/static/img/AS.png",
                    Images: new CardImagesApiResponse(
                        Svg: "https://deckofcardsapi.com/static/img/AS.svg",
                        Png: "https://deckofcardsapi.com/static/img/AS.png"
                    )
                ),
                new CardApiResponse(
                    Code: "KH",
                    Value: "KING",
                    Suit: "HEARTS",
                    Image: "https://deckofcardsapi.com/static/img/KH.png",
                    Images: new CardImagesApiResponse(
                        Svg: "https://deckofcardsapi.com/static/img/KH.svg",
                        Png: "https://deckofcardsapi.com/static/img/KH.png"
                    )
                )
            }
        );

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        var jsonResponse = JsonSerializer.Serialize(expectedResponse, jsonOptions);

        _messageHandler.SetResponse($"deck/{deckId}/draw/?count=2", HttpStatusCode.OK, jsonResponse);

        // Act
        var result = await _service.DrawCardsAsync(deckId, 2);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(deckId, result.DeckId);
        Assert.Equal(2, result.Cards.Length);
        Assert.Equal(50, result.Remaining);
        Assert.Equal("AS", result.Cards[0].Code);
        Assert.Equal("KING", result.Cards[1].Value);
    }

    [Fact]
    public async Task DrawCardsAsync_InvalidDeckId_ReturnsNull()
    {
        // Arrange
        var deckId = "invalid-deck-id";
        _messageHandler.SetResponse($"deck/{deckId}/draw/?count=2", HttpStatusCode.NotFound, "");

        // Act
        var result = await _service.DrawCardsAsync(deckId, 2);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ShuffleDeckAsync_Success_ReturnsDeck()
    {
        // Arrange
        var deckId = "test-deck-123";
        var expectedResponse = new DeckApiResponse(
            Success: true,
            Deck_Id: deckId,
            Shuffled: true,
            Remaining: 52
        );

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        var jsonResponse = JsonSerializer.Serialize(expectedResponse, jsonOptions);

        _messageHandler.SetResponse($"deck/{deckId}/shuffle/", HttpStatusCode.OK, jsonResponse);

        // Act
        var result = await _service.ShuffleDeckAsync(deckId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(deckId, result.DeckId);
        Assert.True(result.Shuffled);
        Assert.Equal(52, result.Remaining);
    }

    [Fact]
    public async Task CreateNewDeckAsync_HttpException_ReturnsNull()
    {
        // Arrange
        _messageHandler.SetException(new HttpRequestException("Network error"));

        // Act
        var result = await _service.CreateNewDeckAsync(1);

        // Assert
        Assert.Null(result);
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        _messageHandler?.Dispose();
    }
}

// Test HttpMessageHandler for mocking HttpClient responses
public class TestHttpMessageHandler : HttpMessageHandler
{
    private readonly Dictionary<string, (HttpStatusCode statusCode, string content)> _responses = new();
    private Exception? _exception;

    public void SetResponse(string relativeUri, HttpStatusCode statusCode, string content)
    {
        _responses[relativeUri] = (statusCode, content);
    }

    public void SetException(Exception exception)
    {
        _exception = exception;
        _responses.Clear();
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_exception != null)
        {
            throw _exception;
        }

        var requestUri = request.RequestUri?.ToString() ?? "";
        var relativeUri = requestUri.Replace("https://deckofcardsapi.com/api/", "");

        if (_responses.TryGetValue(relativeUri, out var response))
        {
            var httpResponse = new HttpResponseMessage(response.statusCode)
            {
                Content = new StringContent(response.content, Encoding.UTF8, "application/json")
            };
            // HttpResponseMessage will be disposed by HttpClient when the response is consumed
            return Task.FromResult(httpResponse);
        }

        var notFoundResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Not Found", Encoding.UTF8, "text/plain")
        };
        // HttpResponseMessage will be disposed by HttpClient when the response is consumed
        return Task.FromResult(notFoundResponse);
    }
}

