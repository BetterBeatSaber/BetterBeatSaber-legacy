using BetterBeatSaber.Server.Models;

namespace BetterBeatSaber.Server.Leaderboards; 

public interface IBaseLeaderboardClient {

    public Task UpdatePlayer(Player player);

}