using BetterBeatSaber.Shared.Enums;

namespace BetterBeatSaber.Server.Integrations; 

public sealed class TwitchIntegration : Integration {

    public TwitchIntegration(IServiceScopeFactory scopeFactory) : base(scopeFactory) { }

    public override IntegrationType Type => IntegrationType.Twitch;
    
    public override string AuthorizeUrl => "https://id.twitch.tv/oauth2/authorize";
    public override string TokenUrl => "https://id.twitch.tv/oauth2/token";

    public override IEnumerable<string> Scopes =>
        new[] {
            "channel:moderate",
            "bits:read",
            "channel:read:charity",
            "channel:read:goals",
            "channel:read:guest_star",
            "channel:read:hype_train",
            "channel:read:polls",
            "channel:read:predictions",
            "channel:read:redemptions",
            "channel:read:subscriptions",
            "channel:read:vips",
            "moderation:read",
            "moderator:read:shoutouts",
            "moderator:manage:shoutouts",
            "chat:edit",
            "chat:read"
        };

}