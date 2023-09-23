using BetterBeatSaber.Server.Leaderboards.ScoreSaber.Models;

namespace BetterBeatSaber.Server.Leaderboards.ScoreSaber.Interfaces; 

public interface IScoreSaberClient {

    public Task<PlayerInformation?> GetPlayerInformation(string id, bool full = true);

    public Task<PlayerInformationList?> GetPlayers(string? search = null, int? page = null, bool? withMetadata = null, params string[] countries);

}