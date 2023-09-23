using BetterBeatSaber.Server.Services.Interfaces;

namespace BetterBeatSaber.Server.Jobs; 

public sealed class RefreshAccessTokensJob : Job<RefreshAccessTokensJob> {

    public RefreshAccessTokensJob(IServiceScopeFactory scopeFactory, ILogger<RefreshAccessTokensJob> logger) : base(scopeFactory, logger) { }

    protected override TimeSpan Delay => TimeSpan.Zero;
    protected override TimeSpan Interval => TimeSpan.FromHours(1);
    
    protected override async Task Run() {

        using var scope = ScopeFactory.CreateScope();

        var integrationService = scope.ServiceProvider.GetService<IIntegrationService>();
        if (integrationService != null)
            await integrationService.RefreshTokens();
        
    }

}