using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Core.UI.Components;
using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;

using HMUI;

using JetBrains.Annotations;

using ModestTree;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace BetterBeatSaber.Core.UI.Main;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

#if DEBUG
[HotReload(RelativePathToLayout = "./MainView.bsml")]
#endif
[ViewDefinition("BetterBeatSaber.Core.UI.Main.MainView.bsml")]
public sealed class MainView : View<MainView> {

    #region UI Params, Components & Objects

    [UsedImplicitly]
    [UIComponent(nameof(ModuleList))]
    public readonly CustomListTableData ModuleList = null!;

    [UsedImplicitly]
    [UIComponent(nameof(FriendRequestList))]
    public readonly CustomListTableData FriendRequestList = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(PlayerList))]
    public readonly CustomListTableData PlayerList = null!;

    [UsedImplicitly]
    [UIComponent("searchBoxContainer")]
    private readonly VerticalLayoutGroup _searchBoxContainer = null!;

    [UsedImplicitly]
    [UIObject("FriendRequestListEmpty")]
    private readonly GameObject _friendListEmpty = null!;

    [UsedImplicitly]
    [UIObject("PlayerSearchStateContainer")]
    private readonly GameObject _playerSearchStateContainer  = null!;

    [UsedImplicitly]
    [UIComponent("PlayerSearchState")]
    private readonly TMP_Text _playerSearchState  = null!;
    
    #endregion

    #region Fields

    public ListData<ModuleManifest, ModuleListCell>? ModuleTable;
    public FriendRequestListTableData? FriendRequestTable;
    public ListData<Player, PlayerListCell>? PlayerTable;

    // ...
    
    private ModuleView? _moduleView;
    
    #endregion

    [UIAction("#post-parse")]
    private void PostParse() {

        ModuleTable = new ListData<ModuleManifest, ModuleListCell>(ModuleList);
        FriendRequestTable = new FriendRequestListTableData(FriendRequestList);
        PlayerTable = new ListData<Player, PlayerListCell>(PlayerList) {
            ItemCellSize = 12f
        };

        StartCoroutine(FetchAvailableModules());

        var searchBox = SearchBox.Create(_searchBoxContainer.transform);
        if (searchBox != null) {
            searchBox.SetPlaceholderText("Search by Name");
            //searchBox.SetKeyboardPositionOffset(new Vector3(-15, -36));
            searchBox.OnValueChanged += UpdateSearchedPlayerList;
        }
        
        // TODO: Implement for Pagnation
        /*var buttons = PlayerList.gameObject.GetComponentsInChildren<NoTransitionsButton>();
        if (buttons is { Length: 2 }) {
            buttons[1].onClick.AddListener(() => {
                Console.WriteLine("DOWNNNN");
                // load more users and refresh table (maybe have to scroll manually where he was?)
            });
        } else {
            Console.WriteLine("No buttons");
        }*/
        
        // Enable Re-Selection of Friend Requests / Players to open their Profiles
        
        FriendRequestTable.SetReselection(true);
        PlayerTable.SetReselection(true);

        FriendManager.Instance.OnFriendRelationshipChanged += OnFriendRelationshipChanged;
        
        UpdateFriendRequestList();
        
        PlayerList.gameObject.SetActive(false);
        _playerSearchStateContainer.SetActive(false);
        _playerSearchState.text = "Enter a name of a player";
        
    }

    private IEnumerator FetchAvailableModules() {
        
        var task = Task.Run(async () => await ModuleManager.FetchAvailableModules());
        yield return new WaitUntil(() => task.IsCompleted);
        
        if (ModuleTable == null)
            yield break;
        
        ModuleTable.Items = task.Result.ToList();
        ModuleTable.Reload();
        
    }

    private void OnFriendRelationshipChanged(Player _, FriendRelationship relationship) {
        if(relationship is FriendRelationship.RequestReceived or FriendRelationship.RequestWithdrawn)
            UpdateFriendRequestList();
    }

    private void UpdateFriendRequestList() {
        
        if (FriendManager.Instance.FriendRequests.Count == 0) {
            FriendRequestList.gameObject.SetActive(false);
            _friendListEmpty.SetActive(true);
        } else {
            FriendRequestList.gameObject.SetActive(true);
            _friendListEmpty.SetActive(false);
        }
        
        FriendRequestTable?.Reload();
        
    }
    
    private void UpdateSearchedPlayerList(string value) {

        if (value.IsEmpty() || value.Length < 2) {
            
            if(PlayerList.gameObject.activeSelf)
                PlayerList.gameObject.SetActive(false);
            
            if(!_playerSearchStateContainer.activeSelf)
                _playerSearchStateContainer.SetActive(true);

            _playerSearchState.text = value.IsEmpty() ? "Enter a name of a player" : "You have to enter at least 2 characters";
            
            return;
        }
        
        StartCoroutine(PerformSearch(value));
        
    }

    private IEnumerator PerformSearch(string query) {
        
        var task = Task.Run(() => PlayerManager.Instance.Search(query));
        yield return new WaitUntil(() => task.IsCompleted);
        
        if (PlayerTable == null)
            yield break;
        
        if (task.Result.IsEmpty()) {
            _playerSearchState.text = "No one found";
            yield break;
        }
        
        if(_playerSearchStateContainer.activeSelf)
            _playerSearchStateContainer.SetActive(false);
        
        if(!PlayerList.gameObject.activeSelf)
            PlayerList.gameObject.SetActive(true);
        
        PlayerTable.Items = task.Result;
        PlayerTable.Reload();
        
    }

    #region UI Actions

    [UIAction(nameof(SelectModule))]
    private void SelectModule([UsedImplicitly] TableView tableView, [UsedImplicitly] int index) {
        
        if (_moduleView == null)
            _moduleView = BeatSaberUI.CreateViewController<ModuleView>();
        
        MainFlowController.Instance.SetRightView(_moduleView);
        
        _moduleView.Populate(ModuleTable?.Items[index]!);
        
    }

    [UIAction(nameof(SelectPlayer))]
    private void SelectPlayer([UsedImplicitly] TableView _, [UsedImplicitly] int index) {
        if(PlayerTable != null)
            this.OpenProfile(PlayerTable.Items[index]);
    }
    
    [UIAction(nameof(SelectFriendRequest))]
    private void SelectFriendRequest([UsedImplicitly] TableView _, [UsedImplicitly] int index) {
        this.OpenProfile(FriendManager.Instance.FriendRequests[index]);
    }

    #endregion

    public class FriendRequestListTableData : ListData<Player, FriendRequestCell> {

        public FriendRequestListTableData(CustomListTableData table, bool reload = false) : base(table, reload) { }

        public override List<Player> Items => FriendManager.Instance.FriendRequests;

        public override float ItemCellSize => 18f;

    }

}