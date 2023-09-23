using System;
using System.Collections.Generic;
using System.Linq;

using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;

using BetterBeatSaber.Core.Api;
using BetterBeatSaber.Core.Config;
using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Game;
using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Core.Network;
using BetterBeatSaber.Core.UI.Friends;
using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;

using JetBrains.Annotations;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace BetterBeatSaber.Core.UI.Main; 

// ReSharper disable UnusedMember.Local

#if DEBUG
[HotReload(RelativePathToLayout = "./LeftView.bsml")]
#endif
public sealed class LeftView : View<LeftView> {

    #region UI Components
    
    [UsedImplicitly]
    [UIComponent(nameof(IntegrationList))]
    public readonly CustomListTableData IntegrationList = null!;

    [UsedImplicitly]
    [UIComponent(nameof(DeleteButton))]
    internal readonly Button DeleteButton = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(State))]
    internal readonly TMP_Text State = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(Ping))]
    internal readonly TMP_Text Ping = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(RTT))]
    internal readonly TMP_Text RTT = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(PacketsReceived))]
    internal readonly TMP_Text PacketsReceived = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(PacketsSent))]
    internal readonly TMP_Text PacketsSent = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(PacketLoss))]
    internal readonly TMP_Text PacketLoss = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(BytesReceived))]
    internal readonly TMP_Text BytesReceived = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(BytesSent))]
    internal readonly TMP_Text BytesSent = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(Version))]
    internal readonly TMP_Text Version = null!;

    #endregion

    #region Options

    public bool CustomTitle {
        // ReSharper disable once UnusedMember.Global
        get => CoreConfig.Instance.CustomTitle;
        set {
            if(CoreConfig.Instance.CustomTitle != value)
                BeatSaber.Restart();
            CoreConfig.Instance.CustomTitle = value;
            CoreConfig.Instance.Save();
        }
    }
    
    public bool ShowFriendList {
        // ReSharper disable once UnusedMember.Global
        get => CoreConfig.Instance.FriendScreen.Enabled;
        set {
            if(CoreConfig.Instance.FriendScreen.Enabled != value)
                BeatSaber.Restart();
            CoreConfig.Instance.FriendScreen.Enabled = value;
            CoreConfig.Instance.Save();
        }
    }

    public bool ShowFriendListHandle {
        // ReSharper disable once Unity.NoNullPropagation
        // ReSharper disable once UnusedMember.Global
        get => FriendsScreen.Instance?.ShowHandle ?? false;
        set {
            if (FriendsScreen.Instance != null)
                FriendsScreen.Instance.ShowHandle = value;
            CoreConfig.Instance.Save();
        }
    }
    
    [UsedImplicitly]
    [UIValue(nameof(FriendListVisibilities))]
    public List<object> FriendListVisibilities => Enum.GetNames(typeof(FloatingViewVisibility)).Cast<object>().ToList();

    [UsedImplicitly]
    [UIAction(nameof(OnChangeFriendListVisibility))]
    public void OnChangeFriendListVisibility(object visibility) =>
        FriendListVisibility = (FloatingViewVisibility) Enum.Parse(typeof(FloatingViewVisibility), visibility.ToString());

    /*[UsedImplicitly]
    [UIAction(nameof(ResetFriendListPositionAndRotation))]
    public void ResetFriendListPositionAndRotation() {
        CoreConfig.Instance.FriendScreen.ResetPositionsAndRotations();
        CoreConfig.Instance.Save();
    }*/
    
    public FloatingViewVisibility FriendListVisibility {
        // ReSharper disable once UnusedMember.Global
        get => CoreConfig.Instance.FriendScreen.Visibility;
        set {
            CoreConfig.Instance.FriendScreen.Visibility = value;
            CoreConfig.Instance.Save();
        }
    }

    #endregion
    
    private CachedListData<IntegrationType, IntegrationCell>? _integrationList;
    
    #region UI Actions

    [UIAction(PostParseEvent)]
    // ReSharper disable once UnusedMember.Local
    private void PostParse() {
        
        _integrationList = new CachedListData<IntegrationType, IntegrationCell>(IntegrationList) {
            Items = new List<IntegrationType> {
                IntegrationType.Discord,
                IntegrationType.Patreon,
                IntegrationType.Twitch
            }
        };
        
        IntegrationManager.Instance.OnIntegrationUpdated += OnIntegrationUpdated;
        
        InvokeRepeating(nameof(UpdateNetworkInformation), .5f, .5f);

    }

    private void OnIntegrationUpdated(IntegrationType type, Integration? integration) {

        if (_integrationList == null)
            return;
        
        if (!_integrationList.Cache.TryGetValue(type, out var cell))
            return;
        
        cell.Populate(type);
        
    }

    [UIAction(nameof(DeleteAccount))]
    private void DeleteAccount() {
        UIManager.Instance.ShowPopup("Are you sure?", string.Join("\n", "This action will delete all your configurations and friends.", "This can't be undone.", "When this is successfully completed your game will close and all data will be deleted.", "If you then start the game again with the mod, a new account will be created."), "Delete", () => {

            DeleteButton.interactable = false;
            
            ApiClient.Instance.DeleteRaw("/players/me").ContinueWith(task => {
                if (task.IsCompleted && task.Result is { IsSuccessStatusCode: true })
                    Application.Quit();
            });
            
        });
    }

    #endregion
    
    private void UpdateNetworkInformation() {
                
        if (NetworkClient.Instance.IsConnected) {

            SetActiveIfNot(Ping.gameObject);
            SetActiveIfNot(RTT.gameObject);
            SetActiveIfNot(PacketsReceived.gameObject);
            SetActiveIfNot(PacketsSent.gameObject);
            SetActiveIfNot(PacketLoss.gameObject);
            SetActiveIfNot(BytesReceived.gameObject);
            SetActiveIfNot(BytesSent.gameObject);

            State.text = $"<b>Connected to {NetworkClient.Instance.ServerName ?? NetworkClient.Instance.EndPoint?.ToString() ?? "Unknown"}</b>";
            Ping.text = $"<b>Ping:</b> {NetworkClient.Instance.Ping ?? -1}ms";
            RTT.text = $"<b>Round-Trip-Time (RTT):</b> {NetworkClient.Instance.RTT ?? -1}ms";
            PacketsReceived.text = $"<b>Packets Received:</b> {NetworkClient.Instance.Statistics?.PacketsReceived}";
            PacketsSent.text = $"<b>Packets Sent:</b> {NetworkClient.Instance.Statistics?.PacketsSent}";
            PacketLoss.text = $"<b>Packet Loss:</b> {NetworkClient.Instance.Statistics?.PacketLoss} ({NetworkClient.Instance.Statistics?.PacketLossPercent}%)";
            BytesReceived.text = $"<b>Bytes Received:</b> {NetworkClient.Instance.Statistics?.BytesReceived.ToBytesCount()}";
            BytesSent.text = $"<b>Bytes Sent:</b> {NetworkClient.Instance.Statistics?.BytesSent.ToBytesCount()}";
            
        } else {
            
            State.text = NetworkClient.Instance.IsReconnecting ? "Reconnecting ..." : "Not connected";
            
            Ping.gameObject.SetActive(false);
            RTT.gameObject.SetActive(false);
            PacketsReceived.gameObject.SetActive(false);
            PacketsSent.gameObject.SetActive(false);
            PacketLoss.gameObject.SetActive(false);
            BytesReceived.gameObject.SetActive(false);
            BytesSent.gameObject.SetActive(false);
            
        }
        
        Version.text = $"<b>Version:</b> {BetterBeatSaber.Version} | {Application.version} | {Application.unityVersion}";
        
    }

}