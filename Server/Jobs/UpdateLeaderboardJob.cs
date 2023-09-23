using BetterBeatSaber.Server.Services.Interfaces;

namespace BetterBeatSaber.Server.Jobs; 

public sealed class UpdateLeaderboardJob : Job<UpdateLeaderboardJob> {

    protected override TimeSpan Delay => TimeSpan.Zero;
    protected override TimeSpan Interval => TimeSpan.FromHours(1);

    public UpdateLeaderboardJob(IServiceScopeFactory scopeFactory, ILogger<UpdateLeaderboardJob> logger) : base(scopeFactory, logger) { }

    protected override async Task Run() {
        
        using var scope = ScopeFactory.CreateScope();
        
        var playerService = scope.ServiceProvider.GetService<IPlayerService>();
        if(playerService != null)
            await playerService.UpdateLeaderboardData();
        
    }

}