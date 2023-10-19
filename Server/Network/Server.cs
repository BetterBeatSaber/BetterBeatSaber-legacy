using BetterBeatSaber.Server.Network.Interfaces;
using BetterBeatSaber.Server.Services;
using BetterBeatSaber.Server.Services.Enums;
using BetterBeatSaber.Server.Services.Interfaces;
using BetterBeatSaber.Server.Twitch.Interfaces;
using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Network.Packets;
using BetterBeatSaber.Twitch.Shared.Enums;
using BetterBeatSaber.Twitch.Shared.Packets;

using LiteNetLib;
using LiteNetLib.Utils;

using Microsoft.Extensions.Options;

using Player = BetterBeatSaber.Server.Models.Player;

namespace BetterBeatSaber.Server.Network; 

public sealed class Server : LifetimeService<Server>, IServer {

    public static readonly string ConnectionKey = Guid.NewGuid().ToString();
    
    private readonly ServerOptions _options;

    private readonly Thread _thread;
    
    public readonly IServiceScopeFactory ScopeFactory;
    
    public EventBasedNetListener Listener { get; private set; }
    public NetPacketProcessor PacketProcessor { get; private set; }
    public NetManager NetManager { get; private set; }

    public List<IConnection> Connections { get; private set; } = new();
    
    public Server(IOptionsMonitor<ServerOptions> config, IServiceScopeFactory scopeFactory, ILogger<Server> logger, IHostApplicationLifetime applicationLifetime) : base(logger, applicationLifetime) {

        _options = config.CurrentValue;
        
        ScopeFactory = scopeFactory;

        Listener = new EventBasedNetListener();
        PacketProcessor = new NetPacketProcessor();
        
        NetManager = new NetManager(Listener);
        
        _thread = new Thread(Run) {
            Name = "Server"
        };

    }

    #region Init & Exit

    public override Task Init() {
        
        Listener.ConnectionRequestEvent += OnConnectionRequestEvent;
        Listener.NetworkReceiveEvent += OnNetworkReceiveEvent;
        Listener.PeerConnectedEvent += OnPeerConnectedEvent;
        Listener.PeerDisconnectedEvent += OnPeerDisconnectedEvent;
        
        // ReSharper disable once AsyncVoidLambda
        PacketProcessor.SubscribeNetSerializable<AuthPacket, Connection>(async (packet, connection) => await OnAuthPacketReceived(packet, connection));
        PacketProcessor.SubscribeNetSerializable<PresencePacket, Connection>(OnPresencePacketReceived);
        PacketProcessor.SubscribeNetSerializable<PresenceStatePacket, Connection>(OnPresenceStatePacketReceived);
        //PacketProcessor.SubscribeNetSerializable<LobbyPacket, Connection>(OnLobbyPacketReceived);
        
        // TODO: Lobbys!!!
        
        PacketProcessor.SubscribeNetSerializable<InitializePacket, Connection>(OnInitializeTwitchPacketReceived);
        
        NetManager.Start(_options.Port);
        
        _thread.Start();
        
        Logger.LogInformation("Listening on {Port}", _options.Port);

        return Task.CompletedTask;

    }

    protected override void Exit() {
        
        PacketProcessor.RemoveSubscription<InitializePacket>();
        
        PacketProcessor.RemoveSubscription<AuthPacket>();
        PacketProcessor.RemoveSubscription<PresencePacket>();
        PacketProcessor.RemoveSubscription<PresenceStatePacket>();
        //PacketProcessor.RemoveSubscription<LobbyPacket>();

        Listener.ConnectionRequestEvent -= OnConnectionRequestEvent;
        Listener.NetworkReceiveEvent -= OnNetworkReceiveEvent;
        Listener.PeerConnectedEvent -= OnPeerConnectedEvent;
        Listener.PeerDisconnectedEvent -= OnPeerDisconnectedEvent;
        
        NetManager.Stop();
        
        Logger.LogInformation("Stopped listening");
        
        _thread.Interrupt();
        
    }

    #endregion
    
    private void Run() {
        while (true) {
            NetManager.PollEvents();
            Thread.Sleep(1000 / 20);
        }
        // ReSharper disable once FunctionNeverReturns
    }
    
    #region Event Handlers
    
    private void OnConnectionRequestEvent(ConnectionRequest request) {
        if (Connections.Count < _options.MaxConnections)
            request.AcceptIfKey(ConnectionKey);
        else
            request.Reject();
    }
    
    private void OnNetworkReceiveEvent(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod) {
        
        var connection = Connections.FirstOrDefault(connection => connection.Peer == peer);
        if (connection == null) {
            Logger.LogWarning("Failed to find Connection for Peer {PeerId}", peer.Id);
            return;
        }
        
        PacketProcessor.ReadAllPackets(reader, connection);
        
    }

    private void OnPeerConnectedEvent(NetPeer peer) {
        var scope = ScopeFactory.CreateScope();
        Connections.Add(ActivatorUtilities.CreateInstance<Connection>(scope.ServiceProvider, this, peer));
    }
    
    private void OnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo) {

        var connection = Connections.FirstOrDefault(connection => connection.Peer == peer);
        if (connection?.Player == null)
            return;
        
        connection.SendPacketToFriends(new PresencePacket {
            Player = connection.Player.ToSharedModel(),
            Presence = new Presence.Offline()
        });

        Connections.Remove(connection);

    }

    #endregion

    #region Packet Handlers

    private async Task OnAuthPacketReceived(AuthPacket packet, Connection connection) {
        
        using var scope = ScopeFactory.CreateScope();
        
        var playerService = scope.ServiceProvider.GetRequiredService<IPlayerService>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        
        var player = await tokenService.GetPlayerFromToken(packet.Session, TokenType.Session);
        if (player is null) {
            
            connection.SendPacket(new AuthResponsePacket {
                ServerName = _options.Name,
                Success = false,
                Reason = "Invalid Session"
            });
            
            connection.Disconnect();
            
            Logger.LogWarning("Authentication failed from {EndPoint} with: {Session}", connection.Peer.EndPoint, packet.Session);
            
            return;
        }
            
        connection.Player = player;
        connection.Friends = await playerService.GetFriends(player);
        
        connection.SendPacket(new AuthResponsePacket {
            ServerName = _options.Name,
            Success = true
        });
        
        foreach (var friendConnection in connection.FriendConnections) {
            
            connection.SendPacket(new PresencePacket {
                Player = friendConnection.Player.ToSharedModel(),
                Presence = friendConnection.Presence
            });

            friendConnection.SendPacket(new PresencePacket {
                Player = player.ToSharedModel(),
                Presence = new Presence.InMenu()
            });
            
            /*if (friendConnection.Lobby != null) {
                connection.SendPacket(new LobbyPacket {
                    Player = friendConnection.Player.ToSharedModel(),
                    Lobby = friendConnection.Lobby
                });
            }*/
            
        }
        
    }

    private static void OnPresencePacketReceived(PresencePacket packet, Connection connection) {

        if (!connection.IsAuthenticated)
            return;
        
        connection.Presence = packet.Presence;
        
        connection.SendPacketToFriends(packet with {
            Player = connection.Player.ToSharedModel()
        });
        
    }
    
    private static void OnPresenceStatePacketReceived(PresenceStatePacket packet, Connection connection) {
        
        if (!connection.IsAuthenticated)
            return;
        
        connection.PresenceState = packet.PresenceState;
        
        connection.SendPacketToFriends(packet with {
            Player = connection.Player.ToSharedModel()
        });
        
    }
    
    /*private static void OnLobbyPacketReceived(LobbyPacket packet, Connection connection) {
        
        connection.Lobby = packet.Lobby;
        
        connection.SendPacketToFriends(packet with {
            Player = connection.Player.ToSharedModel()
        });
        
    }*/
    
    private async void OnInitializeTwitchPacketReceived(InitializePacket packet, Connection connection) {

        if (!connection.IsAuthenticated || connection.Player.Role < PlayerRole.Supporter)
            return;
        
        using var scope = ScopeFactory.CreateScope();

        var twitchService = scope.ServiceProvider.GetService<ITwitchService>()!;
        
        var integrationService = scope.ServiceProvider.GetService<IIntegrationService>()!;
        
        var integration = await integrationService.GetIntegration(connection.Player, IntegrationType.Twitch);
        if (integration == null)
            return;

        var response = await twitchService.TwitchAPI.Helix.Users.GetUsersAsync(accessToken: connection.Player.DecryptToString(integration.AccessToken));

        connection.TwitchChannelId = response.Users.FirstOrDefault()?.Id;
        connection.TwitchChannelName = response.Users.FirstOrDefault()?.Login;
        connection.TwitchFeatures = packet.Features;
        
        if (connection.TwitchChannelName == null)
            return;

        if(packet.Features.HasFlag(FeatureFlag.Chat))
            twitchService.TwitchClient.JoinChannel(connection.TwitchChannelName);

        if (connection.TwitchChannelId == null)
            return;
        
        if(packet.Features.HasFlag(FeatureFlag.Follows))
            twitchService.TwitchPubSub.ListenToFollows(connection.TwitchChannelId);
        
        if(packet.Features.HasFlag(FeatureFlag.Raids))
            twitchService.TwitchPubSub.ListenToRaid(connection.TwitchChannelId);
        
        if(packet.Features.HasFlag(FeatureFlag.Subs))
            twitchService.TwitchPubSub.ListenToSubscriptions(connection.TwitchChannelId);
        
        if(packet.Features.HasFlag(FeatureFlag.ChannelPoints))
            twitchService.TwitchPubSub.ListenToChannelPoints(connection.TwitchChannelId);
        
    }
    
    #endregion

    #region Methods

    public void SendPacketToPlayerIfConnected<T>(Player player, T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : INetSerializable {
        foreach (var connection in Connections.Where(connection => connection.Player.Id == player.Id || connection.Player.Id == player.Id))
            connection.SendPacket(packet, deliveryMethod);
    }

    #endregion
    
    #pragma warning disable CS8618
    
    public class ServerOptions  {

        public string Name { get; set; }
        public string Ip { get; set; }
        public ushort Port { get; set; }
        public int MaxConnections { get; set; }

    }

}