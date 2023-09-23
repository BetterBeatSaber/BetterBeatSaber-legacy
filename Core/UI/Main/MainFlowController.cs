using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;

using BetterBeatSaber.Core.Config;
using BetterBeatSaber.Core.Game;
using BetterBeatSaber.Core.UI.Friends;

using HMUI;

namespace BetterBeatSaber.Core.UI.Main; 

public sealed class MainFlowController : FlowController<MainFlowController> {

    private MainView? _mainView;
    private LeftView? _leftView;

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling) {
        
        if(firstActivation)
            ModuleFlowController.Instantiate();
        
        SetTitle("Better Beat Saber");
        
        showBackButton = true;
        
        if(_mainView == null)
            _mainView = BeatSaberUI.CreateViewController<MainView>();

        if (_leftView == null)
            _leftView = BeatSaberUI.CreateViewController<LeftView>();
        
        ProvideInitialViewControllers(_mainView, _leftView);

    }

    protected override void BackButtonWasPressed(ViewController _) {
        if (ModuleFlowController.RequiresSoftRestart) {
            ModuleFlowController.RequiresSoftRestart = false;
            BeatSaber.Restart();
        } else {
            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
        }
    }

    private static MenuButton? _menuButton;
    
    internal static void Initialize() {
        
        MenuButtons.instance.RegisterButton(_menuButton ??= new MenuButton("<b>Better Beat Saber</b>", "Better Beat Saber", () => {
            BeatSaberUI.MainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf().PresentFlowCoordinator(Instance);
        }));
        
        UIManager.Instance.RegisterFloatingView<FriendsScreen>(CoreConfig.Instance.FriendScreen);
        
    }

}