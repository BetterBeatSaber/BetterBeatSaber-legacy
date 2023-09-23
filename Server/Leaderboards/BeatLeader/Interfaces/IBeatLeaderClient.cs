using BetterBeatSaber.Server.Leaderboards.BeatLeader.Models;

namespace BetterBeatSaber.Server.Leaderboards.BeatLeader.Interfaces; 

public interface IBeatLeaderClient {
    
    public Task<Player?> GetPlayer(string id,
                                   bool stats = false,
                                   bool keepOriginalId = false);
    
    public Task<PlayerList?> GetPlayerList(string? search = null,
                                           string? sortBy = "pp",
                                           uint? page = 1,
                                           uint? count = 50,
                                           byte? order = 0,
                                           IEnumerable<string>? countries = null,
                                           string? mapsType = "ranked",
                                           string? ppType = "general",
                                           bool? friends = false,
                                           string? ppRange = null,
                                           string? scoreRange = null,
                                           string? platform = null,
                                           string? role = null,
                                           string? hmd = null,
                                           IEnumerable<string>? clans = null,
                                           int? activityPeriod = null,
                                           bool? banned = null);

}