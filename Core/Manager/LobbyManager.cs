using System;
using System.Collections.Generic;
using System.Linq;

using BetterBeatSaber.Core.Network;
using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Network.Enums;
using BetterBeatSaber.Shared.Network.Packets;

namespace BetterBeatSaber.Core.Manager; 

public sealed class LobbyManager : Manager<LobbyManager> {

    public Lobby? Lobby { get; private set; }

    public IEnumerable<Player> Players => Lobby?.Players ?? Enumerable.Empty<Player>();

    public bool IsInLobby => Lobby != null;

    #region Events

    public event Action<Lobby>? OnLobbyJoined;
    public event Action<Lobby>? OnLobbyLeft;

    public event Action<Lobby, Player>? OnLobbyPlayerActionReceived;
    
    public event Action<Lobby, Player>? OnPlayerJoined;
    public event Action<Lobby, Player>? OnPlayerLeft;
    public event Action<Lobby, Player>? OnPlayerKicked;
    public event Action<Lobby, Player>? OnPlayerPromoted;

    #endregion

    #region Init & Exit

    public override void Init() {
        NetworkClient.Instance.RegisterPacketHandler<LobbyActionPacket>(OnLobbyActionPacketReceived);
        NetworkClient.Instance.RegisterPacketHandler<LobbyPlayerActionPacket>(OnLobbyPlayerActionPacketReceived);
    }

    public override void Exit() {
        NetworkClient.Instance.UnregisterPacketHandler<LobbyActionPacket>();
        NetworkClient.Instance.UnregisterPacketHandler<LobbyPlayerActionPacket>();
    }

    #endregion

    #region Packet Handlers

    private void OnLobbyActionPacketReceived(LobbyActionPacket packet) {
        
        if (packet.Lobby == null)
            return;
        
        switch (packet.Action) {
            case LobbyAction.Join:
                
                Lobby = packet.Lobby.Value;
                
                OnLobbyJoined?.Invoke(Lobby.Value);
                
                // TODO: patch, send packets etc ...
                
                break;
            case LobbyAction.Leave:
                
                if(Lobby != null)
                    OnLobbyLeft?.Invoke(packet.Lobby.Value);
                
                Lobby = null;
                
                break;
            case LobbyAction.Create:
                // TODO: patch etc...
                break;
            default: throw new ArgumentOutOfRangeException();
        }
        
    }
    
    private void OnLobbyPlayerActionPacketReceived(LobbyPlayerActionPacket packet) {

        if (Lobby == null)
            return;
        
        OnLobbyPlayerActionReceived?.Invoke(Lobby.Value, packet.Player);

        switch (packet.Action) {
            case LobbyPlayerAction.Join:
                OnPlayerJoined?.Invoke(Lobby.Value, packet.Player);
                break;
            case LobbyPlayerAction.Leave:
                OnPlayerLeft?.Invoke(Lobby.Value, packet.Player);
                break;
            case LobbyPlayerAction.Kick:
                OnPlayerKicked?.Invoke(Lobby.Value, packet.Player);
                break;
            case LobbyPlayerAction.Promote:
                OnPlayerPromoted?.Invoke(Lobby.Value, packet.Player);
                break;
            default: throw new ArgumentOutOfRangeException();
        }

    }

    #endregion

    #region Methods

    public bool JoinLobby(Lobby? lobby) => JoinLobby(lobby?.Code);
    
    public bool JoinLobby(string? code) {

        if (code == null || Lobby != null)
            return false;
        
        return NetworkClient.Instance.SendPacket(new LobbyActionPacket {
            Action = LobbyAction.Join,
            LobbyCode = code
        });
        
    }

    public bool CreateLobby() {

        if (Lobby != null)
            return false;

        return NetworkClient.Instance.SendPacket(new LobbyActionPacket {
            Action = LobbyAction.Create
        });

    }

    public bool LeaveLobby() {
        
        if (Lobby == null)
            return false;
        
        NetworkClient.Instance.SendPacket(new LobbyActionPacket {
            Action = LobbyAction.Leave,
            LobbyCode = Lobby.Value.Code
        });
        
        // maybe move to packet handler ?!??!
        // TODO: quit, unpatch etc., hide screen

        return true;

    }

    public void KickPlayer(Player? player) {
        if(Lobby != null && player != null && AuthManager.Instance.CurrentPlayer.Equals(Lobby.Value.Owner))
            NetworkClient.Instance.SendPacket(new LobbyPlayerActionPacket {
                Action = LobbyPlayerAction.Kick,
                Player = player.Value
            });
    }

    public void PromotePlayer(Player? player) {
        if(Lobby != null && player != null && AuthManager.Instance.CurrentPlayer.Equals(Lobby.Value.Owner))
            NetworkClient.Instance.SendPacket(new LobbyPlayerActionPacket {
                Action = LobbyPlayerAction.Promote,
                Player = player.Value
            });
    }
    
    #endregion

}