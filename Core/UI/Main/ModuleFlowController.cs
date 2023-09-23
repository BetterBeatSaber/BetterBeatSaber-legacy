using BeatSaberMarkupLanguage;

using BetterBeatSaber.Core.Config;
using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Manager;

using HMUI;

namespace BetterBeatSaber.Core.UI.Main; 

public sealed class ModuleFlowController : FlowController<ModuleFlowController> {

    protected override string Title => nameof(ModuleFlowController);

    public static bool RequiresSoftRestart = false;

    private Module? _module;
    private View? _view;

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling) {
        
        showBackButton = true;
        
        if(_view != null)
            ProvideInitialViewControllers(_view);

    }

    protected override void BackButtonWasPressed(ViewController _) {
        if(_module is { Config.Updated: true })
            ConfigManager.Instance.SaveConfig(_module.Config);
        MainFlowController.Instance.DismissFlowCoordinator(this);
    }

    // ReSharper disable once MemberCanBeMadeStatic.Global
    public bool HasModule(string moduleId) {
        var module = ModuleManager.Instance.GetModule(moduleId);
        return module != null && (module.FlowControllerType != null || module.ViewType != null);
    }

    public void PresentModule(string moduleId) {

        _module = ModuleManager.Instance.GetModule(moduleId);
        if (_module == null)
            return;

        var flowController = _module.FlowControllerType?.GetInstance<FlowController>();
        if (flowController == null && _module.FlowControllerType != null)
            flowController = UIManager.CreateFlowController(_module.FlowControllerType);

        if (flowController != null) {
            PresentFlowCoordinator(flowController);
            return;
        }

        var view = _module.ViewType?.GetInstance<View>();
        if (view == null && _module.ViewType != null)
            view = (View?) UIManager.CreateViewController(_module.ViewType);

        if (view == null)
            return;

        _view = view;
        
        SetTitle(_module.Manifest.Name);

        MainFlowController.Instance.YoungestChildFlowCoordinatorOrSelf().PresentFlowCoordinator(this);

    }

}