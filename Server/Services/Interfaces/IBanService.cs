namespace BetterBeatSaber.Server.Services.Interfaces; 

public interface IBanService {

    public Task<bool> IsSteamBanned(ulong steamId);
    public Task<bool> IsDiscordBanned(ulong discordId);
    public Task<bool> IsBeatSaverBanned(ulong beatSaverId);

}