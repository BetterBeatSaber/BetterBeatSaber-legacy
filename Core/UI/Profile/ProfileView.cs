using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Core.TextMeshPro;
using BetterBeatSaber.Core.UI.Components;
using BetterBeatSaber.Core.Utilities;
using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Network.Interfaces;

using HMUI;

using JetBrains.Annotations;

using TMPro;

using UnityEngine.UI;

namespace BetterBeatSaber.Core.UI.Profile; 

// ReSharper disable UnusedMember.Local

public sealed class ProfileView : View<ProfileView> {

    #region Header

    [UsedImplicitly]
    [UIComponent(nameof(Avatar))]
    public readonly AvatarComponent Avatar = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(Name))]
    public readonly TMP_Text Name = null!;

    [UsedImplicitly]
    [UIComponent(nameof(Status))]
    public readonly TMP_Text Status = null!;
    
    #region Buttons

    [UsedImplicitly]
    [UIComponent(nameof(FirstButton))]
    public readonly Button FirstButton = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(SecondButton))]
    public readonly Button SecondButton = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(ThirdButton))]
    public readonly Button ThirdButton = null!;

    #endregion
    
    #endregion
    
    #region Status

    [UsedImplicitly]
    [UIComponent(nameof(MapSection))]
    public readonly HorizontalLayoutGroup MapSection = null!;
    
    #region Map
    
    [UsedImplicitly]
    [UIComponent(nameof(MapCover))]
    internal readonly ImageView MapCover = null!;

    [UsedImplicitly]
    [UIComponent(nameof(MapName))]
    internal readonly TMP_Text MapName = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(MapDetails))]
    internal readonly TMP_Text MapDetails = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(MapRankedInfo))]
    internal readonly TMP_Text MapRankedInfo = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(MapPlayerState))]
    internal readonly TMP_Text MapPlayerState = null!;
    
    #endregion

    #endregion
    
    private Player _player;
    
    private PlayerRelationship _relationship;

    private IPresence? _presence;
    private Shared.Models.Lobby? _lobby;

    public void Populate(Player? player) {

        if (player == null)
            return;
        
        _player = player.Value;

        UpdateB();
        
        FriendManager.Instance.OnFriendRelationshipChanged += OnFriendRelationshipChanged;
        FriendManager.Instance.OnFriendStatusUpdated += OnFriendStatusUpdated;
        FriendManager.Instance.OnFriendLobbyUpdated += OnFriendLobbyUpdated;
        FriendManager.Instance.OnFriendPresenceStateUpdated += OnFriendPresenceStateUpdated;
        
        Name.text = player.Value.Name;
        if(player.Value.Flags.HasFlag(PlayerFlag.HasCustomName))
            TextMeshProAddon.Apply(Name);
        
        Avatar.SetPlayer(_player);
        
        UpdateStatus(FriendManager.Instance.GetFriendPresence(player.Value));
        UpdateLobby(FriendManager.Instance.GetFriendLobby(player.Value));
        
    }

    #region Event Handlers

    private void OnFriendRelationshipChanged(Player player, FriendRelationship relationship) {
        UpdateB();
    }
    
    private void OnFriendStatusUpdated(Player player, IPresence? presence) {
        if (player.Id == _player.Id)
            UpdateStatus(presence);
    }
    
    private void OnFriendPresenceStateUpdated(Player player, IPresenceState? presenceState) {
        if (player.Id == _player.Id)
            UpdatePresenceState(presenceState);
    }

    private void OnFriendLobbyUpdated(Player player, Shared.Models.Lobby? lobby) {
        if (player.Id == _player.Id)
            UpdateLobby(lobby);
    }

    #endregion

    #region UI Actions

    [UIAction(nameof(FirstButtonClicked))]
    private void FirstButtonClicked() {
        
        FirstButton.interactable = false;
        
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (_relationship) {
            case PlayerRelationship.Friend:
                FriendManager.Instance.RemoveFriend(_player).ContinueWith(_ => ThreadDispatcher.Enqueue(UpdateB));
                break;
            case PlayerRelationship.ReceivedRequest:
                FriendManager.Instance.AcceptRequest(_player).ContinueWith(_ => ThreadDispatcher.Enqueue(UpdateB));
                break;
            case PlayerRelationship.SentRequest:
                FriendManager.Instance.WithdrawRequest(_player).ContinueWith(_ => ThreadDispatcher.Enqueue(UpdateB));
                break;
            default:
                FriendManager.Instance.SendRequest(_player).ContinueWith(_ => ThreadDispatcher.Enqueue(UpdateB));
                break;
        }
        
    }

    [UIAction(nameof(SecondButtonClicked))]
    private void SecondButtonClicked() {
        if (_relationship == PlayerRelationship.ReceivedRequest) {
            FriendManager.Instance.DeclineRequest(_player).ContinueWith(_ => ThreadDispatcher.Enqueue(UpdateB));
        } else {
            // TODO: Block player
        }
    }
    
    [UIAction(nameof(ThirdButtonClicked))]
    private void ThirdButtonClicked() {
        if (_relationship == PlayerRelationship.ReceivedRequest) {
            // TODO: Block player
        } else if (_relationship == PlayerRelationship.Friend && _lobby != null) {
            LobbyManager.Instance.JoinLobby(_lobby);
        }
    }
    
    #endregion
    
    #region Update

    private void UpdateB() {

        _relationship = PlayerManager.Instance.GetRelationshipWith(_player);

        SetActiveIfNot(ThirdButton.gameObject);
        SetActiveIfNot(SecondButton.gameObject);
        SetActiveIfNot(ThirdButton.gameObject);

        FirstButton.interactable = true;
        SecondButton.interactable = true;
        ThirdButton.interactable = true;
        
        if (_relationship == PlayerRelationship.Self) {
            SetInactiveIfNot(FirstButton.gameObject);
            SetInactiveIfNot(SecondButton.gameObject);
            SetInactiveIfNot(ThirdButton.gameObject);
        } else if (_relationship == PlayerRelationship.Friend) {
            
            FirstButton.SetButtonText("Remove Friend");
            SecondButton.SetButtonText("Block");

            if (_lobby != null) {
                SetActiveIfNot(ThirdButton.gameObject);
                ThirdButton.SetButtonText("Join Lobby");
            } else {
                SetInactiveIfNot(ThirdButton.gameObject);
            }

        } else if(_relationship == PlayerRelationship.ReceivedRequest) {
            
            SetInactiveIfNot(Status.gameObject);
            
            FirstButton.SetButtonText("Accept Request");
            SecondButton.SetButtonText("Decline Request");
            ThirdButton.SetButtonText("Block");
            
        } else if(_relationship == PlayerRelationship.SentRequest) {

            SetInactiveIfNot(Status.gameObject);
            
            FirstButton.SetButtonText("Withdraw Request");
            SecondButton.SetButtonText("Block");
            
            SetInactiveIfNot(ThirdButton.gameObject);
            
        } else {
            
            SetInactiveIfNot(Status.gameObject);
            
            FirstButton.SetButtonText("Send Request");
            SecondButton.SetButtonText("Block");
            
            SetInactiveIfNot(ThirdButton.gameObject);
            
        }

    }
    
    private void UpdateStatus(IPresence? presence) {

        _presence = presence;
        
        UpdateStatusText();

        SetActiveIf(MapSection.gameObject, presence is IPresence.IMap);
        
        if (presence is not IPresence.IMap mapPresence)
            return;
        
        MapName.text = $"{mapPresence.Map.SongAuthor} - {mapPresence.Map.SongName} ({mapPresence.Map.LevelAuthor})";
        
        MapDetails.text = $"{mapPresence.Difficulty.ToString()} | {mapPresence.Difficulty.NotesPerSecond:0.00} NPS | {mapPresence.Difficulty.NoteJumpSpeed:0.0} NJS";
        //MapRankedInfo.text = "RANKEDDDD";

        if (presence is Presence.WatchingReplay watchingReplayPresence)
            MapPlayerState.text = $"Replay of {watchingReplayPresence.User.Name}";

        ThreadDispatcher.Enqueue(MapManager.Instance.DownloadMapCoverAndApply(mapPresence.Map.Hash, MapCover));

    }

    private void UpdatePresenceState(IPresenceState? presenceState) {
        if (presenceState is { Status: Shared.Enums.Status.PlayingMap } and PlayingMapPresenceState mapPresenceState) {
            SetActiveIfNot(MapPlayerState.gameObject);
            MapPlayerState.text = $"{(mapPresenceState.Paused ? "Paused | " : string.Empty)}{mapPresenceState.Rank.ToString()} | {mapPresenceState.Score * 100:0.00} %";
        } else {
            SetInactiveIfNot(MapPlayerState.gameObject);
        }
    }

    private void UpdateLobby(Shared.Models.Lobby? lobby) {
        _lobby = lobby;
        UpdateStatusText();
        UpdateB();
    }
    
    private void UpdateStatusText() =>
        Status.text = _presence.GetStatusText(_lobby);

    #endregion

}