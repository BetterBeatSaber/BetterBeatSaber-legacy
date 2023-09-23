using BeatSaberMarkupLanguage;

using HMUI;

namespace BetterBeatSaber.Core.UI; 

public abstract class FlowController : FlowCoordinator {

    public FlowCoordinator? PreviousFlowController { get; set; }
    
    // ReSharper disable once ReplaceAutoPropertyWithComputedProperty
    protected virtual string Title { get; } = "FlowController";

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling) {
        if(firstActivation)
            SetTitle(Title);
    }

    protected override void BackButtonWasPressed(ViewController _) {
        if(PreviousFlowController != null)
            PreviousFlowController.DismissFlowCoordinator(this);
    }

    public void SetRightView(View view, ViewController.AnimationType animationType = ViewController.AnimationType.In) =>
        SetRightScreenViewController(view, animationType);
    
    public void SetLeftView(View view, ViewController.AnimationType animationType = ViewController.AnimationType.In) =>
        SetLeftScreenViewController(view, animationType);

}

public abstract class FlowController<T> : FlowController where T : FlowController<T> {

    private static T? _instance;
    public static T Instance {
        get {
            if (_instance == null)
                _instance = BeatSaberUI.CreateFlowCoordinator<T>();
            return _instance;
        }
    }

    // ReSharper disable once MemberCanBeProtected.Global
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static T Instantiate() => Instance;

}