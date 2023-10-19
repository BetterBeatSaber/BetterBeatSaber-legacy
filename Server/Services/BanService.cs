using BetterBeatSaber.Server.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace BetterBeatSaber.Server.Services; 

public sealed class BanService : IBanService {

    private readonly AppContext _context;
    
    public BanService(AppContext context) {
        _context = context;
    }

    public async Task<bool> IsSteamBanned(ulong steamId) =>
        await _context.Bans.AnyAsync(ban => ban.SteamId == steamId);

    public async Task<bool> IsDiscordBanned(ulong discordId) =>
        await _context.Bans.AnyAsync(ban => ban.DiscordId == discordId);

    public async Task<bool> IsBeatSaverBanned(ulong beatSaverId) =>
        await _context.Bans.AnyAsync(ban => ban.BeatSaverId == beatSaverId);

}