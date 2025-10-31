using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace test_api.Services;

public class ApiHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // Basic health check - API is running
        var healthData = new Dictionary<string, object>
        {
            { "status", "operational" },
            { "timestamp", DateTime.UtcNow }
        };
        
        return Task.FromResult(
            HealthCheckResult.Healthy("API is operational", healthData));
    }
}

