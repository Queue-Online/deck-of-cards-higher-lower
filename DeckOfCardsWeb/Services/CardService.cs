using DeckOfCardsWeb.Models;
using System.Text.Json;

namespace DeckOfCardsWeb.Services;

public class CardService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public CardService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<DeckResponse?> CreateNewDeckAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("http://localhost:5040/cards/deck/new");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DeckResponse>(json, _jsonOptions);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating deck: {ex.Message}");
        }
        
        return null;
    }

    public async Task<DrawCardsResponse?> DrawCardsAsync(string deckId, int count = 2)
    {
        try
        {
            var url = $"http://localhost:5040/cards/deck/{deckId}/draw?count={count}";
            Console.WriteLine($"Calling URL: {url}");
            
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw JSON response: {json}");
                
                var result = JsonSerializer.Deserialize<DrawCardsResponse>(json, _jsonOptions);
                Console.WriteLine($"Deserialized result: Success={result?.Success}, Cards count={result?.Cards?.Length}");
                
                if (result?.Cards != null)
                {
                    foreach (var card in result.Cards)
                    {
                        Console.WriteLine($"Card: {card.Value} of {card.Suit}, Image: {card.Image}");
                    }
                }
                
                return result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error drawing cards: {ex.Message}");
        }
        
        return null;
    }
}