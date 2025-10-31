using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace DeckOfCardsWeb.Services;

public class StatusService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _baseUrl;
    private readonly ILogger<StatusService> _logger;

    public StatusService(HttpClient httpClient, IConfiguration configuration, ILogger<StatusService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5040";
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<LogsResponse?> GetLogsAsync(int limit = 100)
    {
        try
        {
            // Validate limit to prevent excessive memory usage
            if (limit <= 0 || limit > 1000)
            {
                _logger.LogWarning("Invalid limit parameter: {Limit}. Using default of 100.", limit);
                limit = 100;
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/status/logs?limit={limit}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<LogsResponse>(json, _jsonOptions);
            }
            else
            {
                _logger.LogWarning("Failed to get logs. Status: {StatusCode}", response.StatusCode);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error getting logs");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting logs");
        }
        
        return null;
    }

    public async Task<HealthStatusResponse?> GetHealthStatusAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/status/health");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<HealthStatusResponse>(json, _jsonOptions);
            }
            else
            {
                _logger.LogWarning("Failed to get health status. Status: {StatusCode}", response.StatusCode);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error getting health status");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting health status");
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

