namespace test_api.Services;

public class FeatureToggleService
{
    private readonly IConfiguration _configuration;

    public FeatureToggleService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsObservabilityEnabled()
    {
        return _configuration.GetValue<bool>("FeatureToggles:Observability", false);
    }
}

