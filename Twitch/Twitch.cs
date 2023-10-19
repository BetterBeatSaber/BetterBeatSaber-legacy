using System;
using System.Collections.Generic;

using BetterBeatSaber.Core;
using BetterBeatSaber.Core.Network;
using BetterBeatSaber.Core.UI;
using BetterBeatSaber.Twitch.Config;
using BetterBeatSaber.Twitch.Shared.Enums;
using BetterBeatSaber.Twitch.Shared.Models;
using BetterBeatSaber.Twitch.Shared.Packets;
using BetterBeatSaber.Twitch.UI.Chat;
using BetterBeatSaber.Twitch.UI.Config;

namespace BetterBeatSaber.Twitch;

// ReSharper disable once UnusedType.Global
public sealed class Twitch : Module<Twitch> {

    #region Events

    public event Action<ChatMessage>? OnChatMessageReceived;
    
    public List<ChatMessage> RecentMessages { get; set; } = new();
    public List<User> RecentUsers { get; set; } = new();
    
    #endregion

    #region Fields

    private readonly Dictionary<string, byte[]> _ttsQueue = new();

    #endregion

    public Dictionary<string, List<string>> Voices { get; private set; } = new() {
        { "de-DE", new List<string> { "UwU", "SHEESH" } }
    };
    
    #region Init & Exit

    public override void Init() {
        
        Config = CreateConfig<TwitchConfig>();
        
        RegisterView<MainView>();
        
        UIManager.Instance.RegisterFloatingView<ChatFloatingView>(TwitchConfig.Instance.Chat);

        NetworkClient.Instance.RegisterPacketHandler<ChatMessagePacket>(OnChatMessagePacketReceived);
        
    }

    public override void Exit() {
        UIManager.Instance.UnregisterFloatingView<ChatFloatingView>();
    }

    #endregion

    #region Enable & Disable

    public override void Enable() {
        
        NetworkClient.Instance.RegisterPacketHandler<ChatMessagePacket>(OnChatMessagePacketReceived);
        
        NetworkClient.Instance.OnConnected += () => {
            NetworkClient.Instance.SendPacket(new InitializePacket {
                Features = FeatureFlag.Chat | FeatureFlag.Requests | FeatureFlag.TextToSpeech
            });
        };
        
    }
    
    public override void Disable() {
        NetworkClient.Instance.UnregisterPacketHandler<ChatMessagePacket>();
    }

    #endregion

    #region Packet Handlers

    private void OnChatMessagePacketReceived(ChatMessagePacket packet) {
        RecentMessages.Add(packet.ChatMessage);
        if(!RecentUsers.Contains(packet.ChatMessage.User))
            RecentUsers.Add(packet.ChatMessage.User);
        OnChatMessageReceived?.Invoke(packet.ChatMessage);
        Console.WriteLine(packet.ChatMessage.User.Name + ": " + packet.ChatMessage.Message);
    }
    
    #endregion
    
}