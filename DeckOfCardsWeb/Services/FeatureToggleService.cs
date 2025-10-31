namespace DeckOfCardsWeb.Services;

public class FeatureToggleService
{
    private readonly IConfiguration _configuration;

    public FeatureToggleService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsWelcomeGreetingEnabled()
    {
        return _configuration.GetValue<bool>("FeatureToggles:WelcomeGreeting", false);
    }
}


