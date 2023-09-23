using BetterBeatSaber.Server.Models;
using BetterBeatSaber.Server.Network.Interfaces;
using BetterBeatSaber.Shared.Network.Interfaces;
using BetterBeatSaber.Twitch.Shared.Enums;

using LiteNetLib;
using LiteNetLib.Utils;

namespace BetterBeatSaber.Server.Network; 

public sealed class Connection : IConnection {

    public IServer Server { get; }
    public NetPeer Peer { get; }

    private readonly NetDataWriter _dataWriter = new();

    private Player? _player;
    public Player Player {
        get => _player!;
        set => _player = value;
    }

    public IEnumerable<Player> Friends { get; set; } = Enumerable.Empty<Player>();
    // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
    public IEnumerable<IConnection> FriendConnections => Friends.Select(friend => Server.Connections.FirstOrDefault(connection => connection.Player.Id == friend.Id)).Where(connection => connection != null).Cast<Connection>() ?? Enumerable.Empty<Connection>();

    public IPresence? Presence { get; set; }
    public IPresenceState? PresenceState { get; set; }
    public ILobby? Lobby { get; set; }
    
    public string? TwitchChannelId { get; set; }
    public string? TwitchChannelName { get; set; }
    public FeatureFlag TwitchFeatures { get; set; } = FeatureFlag.None;

    public Connection(IServer server, NetPeer peer) {
        Server = server;
        Peer = peer;
    }

    #region Methods

    public void Disconnect() => Peer.Disconnect();

    public void SendPacket<T>(T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : INetSerializable {
        _dataWriter.Reset();
        Server.PacketProcessor.WriteNetSerializable(_dataWriter, ref packet);
        Peer.Send(_dataWriter, deliveryMethod);
    }

    public void SendPacketToFriends<T>(T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : INetSerializable {
        foreach (var connection in FriendConnections)
            connection.SendPacket(packet, deliveryMethod);
    }

    #endregion

    public void Dispose() {
    }

}