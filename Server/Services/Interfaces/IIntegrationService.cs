using BetterBeatSaber.Server.Integrations;
using BetterBeatSaber.Server.Models;
using BetterBeatSaber.Shared.Enums;

namespace BetterBeatSaber.Server.Services.Interfaces; 

public interface IIntegrationService {

    public IEnumerable<PlayerIntegration> PlayerIntegrations { get; }
    
    public IDictionary<IntegrationType, Integration> Integrations { get; }
    
    public Task<IEnumerable<PlayerIntegration>> GetIntegrations(Player player);
    
    public Task<PlayerIntegration?> GetIntegration(Player player, IntegrationType integrationType);
    
    public Task<PlayerIntegration> AddOrUpdateIntegration(Player player, IntegrationType integrationType, Integration.TokenResponse tokenResponse);
    
    public Task<bool> RemoveIntegration(Player player, IntegrationType integrationType);

    public Task RefreshTokens();

}