using System.Text.Json;

namespace DeckOfCardsWeb.Services;

public class StatusService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public StatusService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<LogsResponse?> GetLogsAsync(int limit = 100)
    {
        try
        {
            var response = await _httpClient.GetAsync($"http://localhost:5040/status/logs?limit={limit}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<LogsResponse>(json, _jsonOptions);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting logs: {ex.Message}");
        }
        
        return null;
    }

    public async Task<HealthStatusResponse?> GetHealthStatusAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("http://localhost:5040/status/health");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<HealthStatusResponse>(json, _jsonOptions);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting health status: {ex.Message}");
        }
        
        return null;
    }
}

public class LogsResponse
{
    public string[]? Logs { get; set; }
    public string? File { get; set; }
    public string? Message { get; set; }
}

public class HealthStatusResponse
{
    public string? Status { get; set; }
    public double? TotalDuration { get; set; }
    public DateTime? Timestamp { get; set; }
    public HealthEntry[]? Entries { get; set; }
}

public class HealthEntry
{
    public string? Name { get; set; }
    public string? Status { get; set; }
    public string? Description { get; set; }
    public double? Duration { get; set; }
    public Dictionary<string, object>? Data { get; set; }
}

