using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;

using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Core.TextMeshPro;
using BetterBeatSaber.Shared.Models;

using TMPro;

using UnityEngine.UI;

namespace BetterBeatSaber.Core.UI.Main;

#if DEBUG
[HotReload(RelativePathToLayout = "./ModuleView.bsml")]
#endif
[ViewDefinition("BetterBeatSaber.Core.UI.Main.ModuleView.bsml")]
public sealed class ModuleView : View<ModuleView> {

    private ModuleManifest _moduleManifest = null!;
    
    private bool _isInstalled;
    
    #region UI Components

    [UIComponent("title")]
    private readonly TMP_Text _title = null!;

    [UIComponent("author")]
    private readonly TMP_Text _author = null!;
    
    [UIComponent("description")]
    private readonly TMP_Text _description = null!;
    
    [UIComponent("install-or-open-settings")]
    private readonly Button _installOrOpenSettingsButton = null!;
    
    [UIComponent("uninstall")]
    private readonly Button _uninstallButton = null!;

    #endregion

    #region UI Actions

    [UIAction("install-or-open-settings")]
    public void InstallOrOpenSettings() {
        if (_isInstalled) {
            ModuleFlowController.Instance.PresentModule(_moduleManifest.Id);
        } else {
            
            SetButtons(false);

            AsyncHelper.RunSync(async () => {
                await ModuleManager.Instance.Install(_moduleManifest.Id);
            });
            
            ModuleFlowController.RequiresSoftRestart = _moduleManifest.RequiresSoftRestart ?? false;
            
            _isInstalled = true;
            
            UpdateButtons();
            
        }
    }

    [UIAction("uninstall")]
    public void Uninstall() {
        
        SetButtons(false);

        ModuleManager.Instance.Uninstall(_moduleManifest.Id);
        
        if (_moduleManifest.RequiresSoftRestart ?? false)
            ModuleFlowController.RequiresSoftRestart = true;
        
        _isInstalled = false;
        
        UpdateButtons();
        
    }

    #endregion
    
    public void Populate(ModuleManifest moduleManifest) {
        
        // TODO: On Install check if any Incompatible mods/modules are installed (also do that with the module manager and display a modal then/at startup)
        
        _moduleManifest = moduleManifest;
        
        _isInstalled = ModuleManager.Instance.IsInstalled(_moduleManifest.Id);
        
        UpdateButtons();
        
        _title.text = $"{_moduleManifest.Name} v{_moduleManifest.Version}";
        _author.text = $"Made by {_moduleManifest.Author}" ?? "<i>Unknown</i>";
        _description.text = _moduleManifest.Description ?? "<i>No description provided.</i>";
        
        TextMeshProAddon.Apply(_title);
        TextMeshProAddon.Apply(_description);
        TextMeshProAddon.Apply(_author);
        
    }

    private void UpdateButtons() {
        
        if (!_installOrOpenSettingsButton.interactable)
            _installOrOpenSettingsButton.interactable = true;
        
        if (!_uninstallButton.interactable)
            _uninstallButton.interactable = true;
        
        if (_isInstalled) {
            
            if(!_uninstallButton.gameObject.activeSelf)
                _uninstallButton.gameObject.SetActive(true);

            _installOrOpenSettingsButton.SetButtonText("Open Settings");
            _installOrOpenSettingsButton.interactable = ModuleFlowController.Instance.HasModule(_moduleManifest.Id);
            
        } else {
            _installOrOpenSettingsButton.SetButtonText("Install");
            //_installOrOpenSettingsButton.interactable = !_moduleManifest.ConflictsWithSomething();
            _uninstallButton.gameObject.SetActive(false);
        }
        
    }
    
    private bool SetButtons(bool value) {
        _installOrOpenSettingsButton.interactable = value;
        _uninstallButton.interactable = value;
        return value;
    }
    
}