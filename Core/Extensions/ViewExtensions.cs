using BeatSaberMarkupLanguage;

using BetterBeatSaber.Core.UI;
using BetterBeatSaber.Core.UI.Profile;
using BetterBeatSaber.Shared.Models;

using HMUI;

using UnityEngine;

namespace BetterBeatSaber.Core.Extensions; 

public static class ViewExtensions {

    public static void OpenProfile(this View view, Player player, bool useMainScreen = false) =>
        OpenProfile(view, player, (useMainScreen ? BeatSaberUI.MainFlowCoordinator.GetField<FlowCoordinator, ViewController>("_providedMainViewController")?.gameObject : view.gameObject) ?? view.gameObject);

    public static void OpenProfile(this View view, Player player, GameObject parent) {

        var profileView = ProfileView.Instance;
        if (profileView == null)
            profileView = BeatSaberUI.CreateViewController<ProfileView>();

        var parserParams = BSMLParser.instance.Parse(typeof(ProfileView).ReadViewDefinition(), parent, profileView);
        
        profileView.Populate(player);
        
        parserParams.EmitEvent("ShowProfile");
        
    }
    
}