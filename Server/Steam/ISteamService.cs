using BetterBeatSaber.Server.Steam.Models;

namespace BetterBeatSaber.Server.Steam; 

public interface ISteamService {

    public Task<(AuthResponseParams?, AuthResponseError?)> Authenticate(uint appId, string ticket);
    public Task<IEnumerable<ProfileResponsePlayer>> GetPlayerSummaries(params ulong[] steamIds);
    public Task<ProfileResponsePlayer?> GetPlayerSummary(ulong steamId);

}