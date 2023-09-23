using BetterBeatSaber.Shared.Enums;

namespace BetterBeatSaber.Server.Integrations; 

public sealed class DiscordIntegration : Integration {

    public DiscordIntegration(IServiceScopeFactory scopeFactory) : base(scopeFactory) { }

    public override IntegrationType Type => IntegrationType.Discord;
    
    public override string AuthorizeUrl => "https://discord.com/oauth2/authorize";
    public override string TokenUrl => "https://discord.com/api/oauth2/token";
    
    public override IEnumerable<string> Scopes => new[] { "identify" };

}