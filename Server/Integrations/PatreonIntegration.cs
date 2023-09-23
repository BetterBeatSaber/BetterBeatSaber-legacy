using BetterBeatSaber.Shared.Enums;

namespace BetterBeatSaber.Server.Integrations; 

public sealed class PatreonIntegration : Integration {

    public PatreonIntegration(IServiceScopeFactory scopeFactory) : base(scopeFactory) { }

    public override IntegrationType Type => IntegrationType.Patreon;
    
    public override string AuthorizeUrl => "https://www.patreon.com/oauth2/authorize";
    public override string TokenUrl => "https://www.patreon.com/api/oauth2/token";
    
    public override IEnumerable<string> Scopes => new[] { "users", "pledges-to-me" };

}