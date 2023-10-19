using System.Collections.Generic;
using System.Linq;

using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;

using BetterBeatSaber.Core.Config;
using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Game;
using BetterBeatSaber.Core.Game.Enums;
using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Network.Interfaces;

using HMUI;

using JetBrains.Annotations;

using UnityEngine;

namespace BetterBeatSaber.Core.UI.Friends; 

public sealed class FriendsScreen : FloatingView<FriendsScreen> {

    public override Vector2 ScreenSize { get; } = new(100f, 80f);
    
    #region Components
    
    [UsedImplicitly]
    [UIComponent(nameof(FriendList))]
    public readonly CustomListTableData FriendList = null!;

    #endregion
    
    #region Properties & Fields

    private FriendListTableData? _cachedTableData;

    #endregion

    #region UI Actions

    [UIAction(PostParseEvent)]
    // ReSharper disable once UnusedMember.Local
    private void PostParse() {

        _cachedTableData = new FriendListTableData(FriendList, true);

        FriendManager.Instance.OnFriendRelationshipChanged += OnFriendRelationshipChanged;
        FriendManager.Instance.OnFriendStatusUpdated += OnFriendStatusUpdated;
        
        _cachedTableData.SetReselection(true);
        
    }

    [UIAction(nameof(SelectFriend))]
    public void SelectFriend(TableView tableView, int idx) {
        if(BeatSaber.ActiveGenericScene == GenericScene.Menu)
            this.OpenProfile(FriendManager.Instance.Friends[idx], true);
    }
    
    #endregion

    #region Unity Event Functions

    protected override void OnDestroy() {
        FriendManager.Instance.OnFriendRelationshipChanged -= OnFriendRelationshipChanged;
        FriendManager.Instance.OnFriendStatusUpdated -= OnFriendStatusUpdated;
    }

    #endregion
    
    #region Event Handlers

    private void OnFriendRelationshipChanged(Player player, FriendRelationship relationship) {

        if (_cachedTableData == null || (relationship != FriendRelationship.FriendRemoved && relationship != FriendRelationship.RequestAccepted))
            return;
        
        if(relationship == FriendRelationship.FriendRemoved)
            _cachedTableData.Cache.Remove(_cachedTableData.Cache.FirstOrDefault(p => p.Key.Id == player.Id).Key);
        
        _cachedTableData.Reload(); 
        
    }

    private void OnFriendStatusUpdated(Player player, IPresence? presence) {

        if (_cachedTableData == null)
            return;
        
        if (!_cachedTableData.Cache.TryGetValue(player, out var cell))
            return;
        
        cell.UpdateStatus(presence, player.GetLobby());
        
    }

    #endregion

    public override void SaveConfig() => CoreConfig.Instance.Save();

    private class FriendListTableData : CachedListData<Player, FriendListCell> {

        public FriendListTableData(CustomListTableData table, bool reloadData = false) : base(table, reloadData) { }

        public override List<Player> Items => FriendManager.Instance.Friends.ToList();

    }

}