using System;
using System.Collections.Generic;

using BetterBeatSaber.Core.Manager.Service;
using BetterBeatSaber.Core.Network;
using BetterBeatSaber.Twitch.Shared.Enums;
using BetterBeatSaber.Twitch.Shared.Models;
using BetterBeatSaber.Twitch.Shared.Packets;

namespace BetterBeatSaber.Twitch; 

public sealed class TwitchService : Service<TwitchService> {

    #region Events

    public event Action<ChatMessage>? OnChatMessageReceived;
    public List<ChatMessage> LatestMessages { get; set; } = new();

    #endregion
    
    #region Init & Exit

    public override void Init() {
        NetworkClient.Instance.RegisterPacketHandler<ChatMessagePacket>(OnChatMessagePacketReceived);
    }
    
    public override void Exit() {
        NetworkClient.Instance.UnregisterPacketHandler<ChatMessagePacket>();
    }
    
    #endregion
    
    #region Enable & Disable

    public override void Enable() {
        NetworkClient.Instance.OnConnected += () => {
            NetworkClient.Instance.SendPacket(new InitializePacket {
                Features = FeatureFlag.Chat | FeatureFlag.Requests
            });
        };
    }

    public override void Disable() {
    }

    #endregion

    #region Packet Handlers

    private void OnChatMessagePacketReceived(ChatMessagePacket packet) {
        LatestMessages.Add(packet.ChatMessage);
        OnChatMessageReceived?.Invoke(packet.ChatMessage);
    }

    #endregion
    
}