using System.Collections.Generic;
using System.Linq;

using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;

using BetterBeatSaber.Core.UI;
using BetterBeatSaber.Core.UI.Main;
using BetterBeatSaber.Core.UI.StringList;
using BetterBeatSaber.Twitch.Config;
using BetterBeatSaber.Twitch.Shared.Models;

using HMUI;

using JetBrains.Annotations;

namespace BetterBeatSaber.Twitch.UI.Config; 

public sealed class MainView : View<MainView> {

    public static List<User> Users {
        get {
            return TwitchConfig.Instance.Users.Select(keyValuePair => new User {
                Id = keyValuePair.Key,
                Name = keyValuePair.Value.CustomName
            }).Concat(Twitch.Instance.RecentUsers).ToList();
        }
    }
    
    [UsedImplicitly]
    [UIComponent(nameof(UserList))]
    public readonly CustomListTableData UserList = null!;
    
    public StringList? UserTable;

    private UserView? _userView;
    
    [UIAction(PostParseEvent)]
    public void PostParse() {
        UserTable = new StringList(UserList) {
            Items = Users.Select(user => user.Name).ToList()
        };
        //_userView = BeatSaberUI.CreateViewController<UserView>();
    }

    [UIAction(nameof(SelectUser))]
    public void SelectUser([UsedImplicitly] TableView tableView, [UsedImplicitly] int index) {
        
        if (_userView == null)
            return;

        _userView.User = Users[index];
        
        ModuleFlowController.Instance.SetRightView(_userView);
        
    }
    
}