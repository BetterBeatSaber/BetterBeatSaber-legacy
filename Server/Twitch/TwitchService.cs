using BetterBeatSaber.Server.Network.Interfaces;
using BetterBeatSaber.Server.Services;
using BetterBeatSaber.Server.Services.Enums;
using BetterBeatSaber.Server.Services.Interfaces;
using BetterBeatSaber.Server.Twitch.Interfaces;
using BetterBeatSaber.Twitch.Shared.Enums;
using BetterBeatSaber.Twitch.Shared.Models;
using BetterBeatSaber.Twitch.Shared.Packets;

using TwitchLib.Api;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Interfaces;

using ChatMessage = BetterBeatSaber.Twitch.Shared.Models.ChatMessage;

namespace BetterBeatSaber.Server.Twitch; 

public sealed class TwitchService : LifetimeService<TwitchService>, ITwitchService {

    private readonly IServiceScopeFactory _scopeFactory;
    
    public ITwitchClient TwitchClient { get; }
    public ITwitchPubSub TwitchPubSub { get; }
    public ITwitchAPI TwitchAPI { get; }
    
    public TwitchService(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<TwitchService> logger, IHostApplicationLifetime applicationLifetime) : base(logger, applicationLifetime) {
        
        _scopeFactory = scopeFactory;

        TwitchClient = new TwitchClient();
        TwitchPubSub = new TwitchPubSub();
        TwitchAPI = new TwitchAPI();

        TwitchAPI.Settings.ClientId = configuration.GetValue<string>("Twitch:ClientId");
        
        TwitchClient.Initialize(new ConnectionCredentials(configuration.GetValue<string>("Twitch:Login"), configuration.GetValue<string>("Twitch:AuthToken")));
        
    }
    
    public override Task Init() {
        
        TwitchClient.OnMessageReceived += OnMessageReceived;
        
        TwitchClient.Connect();
        TwitchPubSub.Connect();

        return Task.CompletedTask;
        
    }

    protected override void Exit() {
        
        TwitchClient.OnMessageReceived -= OnMessageReceived;
        
        TwitchPubSub.Disconnect();
        TwitchClient.Disconnect();
        
    }

    #region Event Handlers

    #region IRC

    private void OnMessageReceived(object? _, OnMessageReceivedArgs args) {

        using var scope = _scopeFactory.CreateScope();
        
        var server = scope.ServiceProvider.GetService<IServer>();
        var tokenService = scope.ServiceProvider.GetService<ITokenService>();
        
        if (server == null || tokenService == null)
            return;

        var chatMessage = new ChatMessage {
            Id = args.ChatMessage.Id,
            Channel = args.ChatMessage.Channel,
            User = new User {
                Id = args.ChatMessage.UserId,
                Name = args.ChatMessage.Username
            },
            Message = args.ChatMessage.Message
        };

        foreach (var connection in server.Connections.Where(connection => connection.TwitchChannelName == args.ChatMessage.Channel)) {
            connection.SendPacket(new ChatMessagePacket {
                ChatMessage = chatMessage,
                TextToSpeechToken = connection.TwitchFeatures.HasFlag(FeatureFlag.TextToSpeech) ? tokenService.CreateToken(connection.Player, TokenType.TwitchTextToSpeech) : null
            });
        }
        
    }

    #endregion
    
    #region Pub Sub

    

    #endregion

    #endregion

    private void Fff(string id) {
        
    }

}