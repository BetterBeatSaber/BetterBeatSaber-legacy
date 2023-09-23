using BetterBeatSaber.Server.Integrations;
using BetterBeatSaber.Server.Models;
using BetterBeatSaber.Server.Services.Interfaces;
using BetterBeatSaber.Shared.Enums;

using Microsoft.EntityFrameworkCore;

namespace BetterBeatSaber.Server.Services; 

public sealed class IntegrationService : IIntegrationService {

    private readonly AppContext _context;

    public IDictionary<IntegrationType, Integration> Integrations { get; }

    public IEnumerable<PlayerIntegration> PlayerIntegrations => _context.PlayerIntegrations;
    
    public IntegrationService(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        DiscordIntegration discordIntegration,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        PatreonIntegration patreonIntegration,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        TwitchIntegration twitchIntegration,
        AppContext context
        ) {
        
        _context = context;
        
        Integrations = new Dictionary<IntegrationType, Integration> {
            { IntegrationType.Discord, discordIntegration },
            { IntegrationType.Patreon, patreonIntegration },
            { IntegrationType.Twitch, twitchIntegration }
        };
        
    }

    public async Task<IEnumerable<PlayerIntegration>> GetIntegrations(Player player) =>
        await _context.PlayerIntegrations.Where(integration => integration.Player == player).ToListAsync();

    public async Task<PlayerIntegration?> GetIntegration(Player player, IntegrationType integrationType) =>
        await _context.PlayerIntegrations.FirstOrDefaultAsync(connection => connection.Player == player && connection.Type == integrationType);

    public async Task<PlayerIntegration> AddOrUpdateIntegration(Player player, IntegrationType integrationType, Integration.TokenResponse tokenResponse) {

        var playerIntegration = await _context.PlayerIntegrations.FirstOrDefaultAsync(integration => integration.Player == player && integration.Type == integrationType);
        if (playerIntegration != null) {

            playerIntegration.AccessToken = player.Encrypt(tokenResponse.AccessToken);
            playerIntegration.RefreshToken = player.Encrypt(tokenResponse.RefreshToken);
            playerIntegration.TokenType = tokenResponse.TokenType;
            playerIntegration.ExpiresAt = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn);
            
            await _context.SaveChangesAsync();

            return playerIntegration;
        }
        
        playerIntegration = new PlayerIntegration {
            Id = Guid.NewGuid(),
            Player = player,
            Type = integrationType,
            AccessToken = player.Encrypt(tokenResponse.AccessToken),
            RefreshToken = player.Encrypt(tokenResponse.RefreshToken),
            TokenType = tokenResponse.TokenType,
            ExpiresAt = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn)
        };

        await _context.PlayerIntegrations.AddAsync(playerIntegration);
        await _context.SaveChangesAsync();
        
        return playerIntegration;
        
    }

    public async Task<bool> RemoveIntegration(Player player, IntegrationType integrationType) {

        var playerIntegration = await GetIntegration(player, integrationType);
        if (playerIntegration == null)
            return false;
        
        _context.PlayerIntegrations.Remove(playerIntegration);
        
        await _context.SaveChangesAsync();
        
        return true;
        
    }

    public async Task RefreshTokens() {
        foreach (var integration in _context.PlayerIntegrations.Where(integration => integration.ExpiresAt < DateTime.Now).Include(integration => integration.Player)) {
            
            var tokens = await Integrations[integration.Type].RefreshToken(integration.Player.DecryptToString(integration.RefreshToken));
            if(tokens == null)
                continue;

            integration.TokenType = tokens.TokenType;
            integration.AccessToken = integration.Player.Encrypt(tokens.AccessToken);
            integration.RefreshToken = integration.Player.Encrypt(tokens.RefreshToken);
            integration.ExpiresAt = DateTime.Now.AddSeconds(tokens.ExpiresIn);
            
            await _context.SaveChangesAsync();
            
        }
    }

}