using System.Collections.Generic;
using System.Linq;

using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Shared.Models;

using HMUI;

using JetBrains.Annotations;

namespace BetterBeatSaber.Core.UI.Lobby; 

public sealed class LobbyFloatingView : View<LobbyFloatingView>, View.IPostParseEventHandler {

    [UsedImplicitly]
    [UIComponent(nameof(PlayerList))]
    public readonly CustomListTableData PlayerList = null!;
    
    private PlayerListData? _playerListData;

    #region Unity & BSML Event Handlers

    [UIAction(PostParseEvent)]
    public void PostParse() {
        
        _playerListData = new PlayerListData(PlayerList);
        
        LobbyManager.Instance.OnLobbyPlayerActionReceived += OnLobbyPlayerActionReceived;
        
    }

    protected override void OnDeactivate() {
        LobbyManager.Instance.OnLobbyPlayerActionReceived -= OnLobbyPlayerActionReceived;
    }

    #endregion

    #region Event Handlers

    private void OnLobbyPlayerActionReceived(Shared.Models.Lobby _, Player __) => _playerListData?.Reload();

    #endregion

    [UIAction(nameof(SelectPlayer))]
    public void SelectPlayer(TableView tableView, int index) {
        if(_playerListData != null)
            this.OpenProfile(_playerListData.Items[index]);
    }
    
    public sealed class PlayerListData : ListData<Player, LobbyPlayerListCell> {

        public PlayerListData(CustomListTableData table, bool reload = true) : base(table, reload) { }

        public override List<Player> Items => LobbyManager.Instance.Players.ToList();

    }

}