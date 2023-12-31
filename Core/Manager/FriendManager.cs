﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BetterBeatSaber.Core.Api;
using BetterBeatSaber.Core.Network;
using BetterBeatSaber.Core.Utilities;
using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Network.Interfaces;
using BetterBeatSaber.Shared.Network.Packets;

namespace BetterBeatSaber.Core.Manager; 

// ReSharper disable MemberCanBeMadeStatic.Global

public sealed class FriendManager : Manager<FriendManager> {

    public event Action<Player, FriendRelationship>? OnFriendRelationshipChanged;
    public event Action<Player, IPresence?>? OnFriendStatusUpdated;
    public event Action<Player, IPresenceState?>? OnFriendPresenceStateUpdated;
    public event Action<Player, Lobby?>? OnFriendLobbyUpdated;
    
    public List<Player> Friends { get; private set; } = new();
    public List<Player> FriendRequests { get; private set; } = new();
    public List<Player> SentFriendRequests { get; private set; } = new();

    public bool IsLoading { get; private set; } = true;
    
    public Dictionary<ulong, IPresence?> FriendPresences { get; } = new();
    public Dictionary<ulong, IPresenceState?> FriendPresenceStates { get; } = new();
    public Dictionary<ulong, Lobby?> FriendLobbies { get; } = new();

    #region Init & Exit

    public override void Init() {
        
        NetworkClient.Instance.RegisterPacketHandler<FriendRelationshipPacket>(OnFriendRelationshipPacketReceived);
        NetworkClient.Instance.RegisterPacketHandler<PresencePacket>(OnPresencePacketReceived);
        NetworkClient.Instance.RegisterPacketHandler<PresenceStatePacket>(OnPresenceStatePacketReceived);
        //NetworkClient.Instance.RegisterPacketHandler<LobbyPacket>(OnLobbyPacketReceived);
        
        AuthManager.Instance.OnAuthenticated += OnAuthenticated;

    }

    public override void Exit() {
        
        AuthManager.Instance.OnAuthenticated -= OnAuthenticated;
        
        NetworkClient.Instance.UnregisterPacketHandler<FriendRelationshipPacket>();
        NetworkClient.Instance.UnregisterPacketHandler<PresencePacket>();
        //NetworkClient.Instance.UnregisterPacketHandler<LobbyPacket>();
        
    }

    // Maybe Combine to one Request?
    private IEnumerator Fetch() {

        IsLoading = true;
        
        var request = new ApiRequest<List<Player>>("/friends");
        yield return request.Send();
        Friends = request.Response ?? Enumerable.Empty<Player>().ToList();
        
        request = new ApiRequest<List<Player>>("/friends/requests");
        yield return request.Send();
        FriendRequests = request.Response ?? Enumerable.Empty<Player>().ToList();

        request = new ApiRequest<List<Player>>("/friends/requests/sent");
        yield return request.Send();
        SentFriendRequests = request.Response ?? Enumerable.Empty<Player>().ToList();

        IsLoading = false;

    }

    #endregion

    #region Event Handlers

    private void OnAuthenticated() =>
        ThreadDispatcher.Enqueue(Fetch());
    
    private void OnFriendRelationshipPacketReceived(FriendRelationshipPacket packet) {
        
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (packet.Relationship) {
            case FriendRelationship.FriendRemoved:
                Friends.RemoveAll(friend => friend.Id == packet.Player.Id);
                break;
            case FriendRelationship.RequestReceived:
                FriendRequests.Add(packet.Player);
                break;
            case FriendRelationship.RequestAccepted:
                FriendRequests.RemoveAll(friendRequest => friendRequest.Id == packet.Player.Id);
                Friends.Add(packet.Player);
                break;
            case FriendRelationship.RequestDeclined:
                break;
            case FriendRelationship.RequestWithdrawn:
                FriendRequests.RemoveAll(friendRequest => friendRequest.Id == packet.Player.Id);
                break;
        }
        
        OnFriendRelationshipChanged?.Invoke(packet.Player, packet.Relationship);
        
    }
    
    private void OnPresencePacketReceived(PresencePacket packet) {
        
        if (packet.Player == null)
            return;

        FriendPresences[packet.Player.Value.Id] = packet.Presence;
        
        OnFriendStatusUpdated?.Invoke(packet.Player.Value, packet.Presence);
        
    }
    
    private void OnPresenceStatePacketReceived(PresenceStatePacket packet) {
        
        if (packet.Player == null)
            return;
        
        if(packet.PresenceState is PlayingMapPresenceState playingMapPresenceState)
            Console.WriteLine(packet.Player.Value.Name + ": " + playingMapPresenceState.Paused + " - " + playingMapPresenceState.Rank + " - " + playingMapPresenceState.Score);
        
        FriendPresenceStates[packet.Player.Value.Id] = packet.PresenceState;

        OnFriendPresenceStateUpdated?.Invoke(packet.Player.Value, packet.PresenceState);
        
    }
    
    /*private void OnLobbyPacketReceived(LobbyPacket packet) {
        
        if (packet.Player == null)
            return;
        
        FriendLobbies[packet.Player.Value.Id] = packet.Lobby;
        
        OnFriendLobbyUpdated?.Invoke(packet.Player.Value, packet.Lobby);
        OnFriendStatusUpdated?.Invoke(packet.Player.Value, GetFriendPresence(packet.Player.Value), packet.Lobby);

    }*/
    
    #endregion

    #region Methods

    public bool IsFriend(Player player) =>
        Friends.Any(friend => friend.Id == player.Id);

    public bool HasReceivedRequestFrom(Player player) =>
        FriendRequests.Any(request => request.Id == player.Id);

    public bool HasSentRequestTo(Player player) =>
        SentFriendRequests.Any(sentRequest => sentRequest.Id == player.Id);
    
    public async Task<bool> RemoveFriend(Player player) {

        var response = await ApiClient.Instance.DeleteRaw($"/friends/{player.Id}");
        if (response is not { IsSuccessStatusCode: true })
            return false;

        Friends.Remove(player);
        OnFriendRelationshipChanged?.Invoke(player, FriendRelationship.FriendRemoved);
        
        return true;
        
    }

    public async Task<bool> SendRequest(Player player) {
       
        var response = await ApiClient.Instance.Post($"/friends/{player.Id}");
        if (response is not { IsSuccessStatusCode: true })
            return false;
        
        SentFriendRequests.Add(player);
        
        return true;
        
    }

    public async Task<bool> AcceptRequest(Player player) {
        
        var response = await ApiClient.Instance.Post($"/friends/{player.Id}");
        if (response is not { IsSuccessStatusCode: true })
            return false;

        FriendRequests.Remove(player);
        Friends.Add(player);
        
        OnFriendRelationshipChanged?.Invoke(player, FriendRelationship.RequestAccepted);
        
        return true;
        
    }

    public async Task<bool> DeclineRequest(Player player) {
        
        var response = await ApiClient.Instance.DeleteRaw($"/friends/{player.Id}");
        if (response is not { IsSuccessStatusCode: true })
            return false;

        FriendRequests.Remove(player);
        OnFriendRelationshipChanged?.Invoke(player, FriendRelationship.RequestDeclined);
        
        return true;
        
    }
    
    public async Task<bool> WithdrawRequest(Player player) {
        
        var response = await ApiClient.Instance.DeleteRaw($"/friends/{player.Id}");
        if (response is not { IsSuccessStatusCode: true })
            return false;

        SentFriendRequests.Remove(player);
        OnFriendRelationshipChanged?.Invoke(player, FriendRelationship.RequestWithdrawn);
        
        return true;
        
    }
    
    public IPresence? GetFriendPresence(Player player) =>
        FriendPresences.TryGetValue(player.Id, out var presence) ? presence : null;
    
    public IPresenceState? GetFriendPresenceState(Player player) =>
        FriendPresenceStates.TryGetValue(player.Id, out var presenceState) ? presenceState : null;

    public Lobby? GetFriendLobby(Player player) =>
        FriendLobbies.TryGetValue(player.Id, out var lobby) ? lobby : null;
    
    #endregion

}