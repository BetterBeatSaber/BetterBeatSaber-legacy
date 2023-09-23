﻿using BetterBeatSaber.Server.Models;
using BetterBeatSaber.Shared.Network.Interfaces;
using BetterBeatSaber.Twitch.Shared.Enums;

using LiteNetLib;
using LiteNetLib.Utils;

namespace BetterBeatSaber.Server.Network.Interfaces; 

public interface IConnection : IDisposable {

    public IServer Server { get; }
    public NetPeer Peer { get; }
    
    public Player Player { get; }
    
    public IEnumerable<Player> Friends { get; }
    public IEnumerable<IConnection> FriendConnections { get; }

    public IPresence? Presence { get; }
    public IPresenceState? PresenceState { get; }
    public ILobby? Lobby { get; }
    
    public string? TwitchChannelId { get; set; }
    public string? TwitchChannelName { get; set; }
    public FeatureFlag TwitchFeatures { get; set; }
    
    #region Methods

    public void Disconnect();
    
    public void SendPacket<T>(T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : INetSerializable;
    public void SendPacketToFriends<T>(T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : INetSerializable;

    #endregion

}