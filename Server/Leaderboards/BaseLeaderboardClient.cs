using BetterBeatSaber.Server.Models;

namespace BetterBeatSaber.Server.Leaderboards;

public abstract class BaseLeaderboardClient<T> : IBaseLeaderboardClient where T : BaseLeaderboardClient<T> {

    protected readonly HttpClient HttpClient;
    
    protected BaseLeaderboardClient(HttpClient httpClient) {
        HttpClient = httpClient;
    }

    public abstract Task UpdatePlayer(Player player);

}