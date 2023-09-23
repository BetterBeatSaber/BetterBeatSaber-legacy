using System;

using BetterBeatSaber.Core.Config;
using BetterBeatSaber.Core.Manager.Service;
using BetterBeatSaber.Core.UI;
using BetterBeatSaber.Core.Zenject;
using BetterBeatSaber.Shared.Models;

using HarmonyLib;

using IPA.Logging;

using JetBrains.Annotations;

namespace BetterBeatSaber.Core; 

// ReSharper disable MemberCanBeMadeStatic.Global

public abstract class Module : Interfaces.IInitializable, Interfaces.IEnableable {

    // Will be injected
    [UsedImplicitly]
    public readonly ModuleManifest Manifest = null!;
    
    // Will be injected
    [UsedImplicitly]
    public readonly Logger Logger = null!;

    [UsedImplicitly]
    public readonly bool IsLocal;
    
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public Config.Config? Config;

    internal Type? ViewType { get; private set; }
    internal Type? FlowControllerType { get; private set; }
    
    private Harmony? _harmony;
    
    public string Id => Manifest.Id;

    #region Init & Exit

    public virtual void Init() { }
    public virtual void Exit() { }

    #endregion

    #region Enable & Disable

    public virtual void Enable() { }
    public virtual void Disable() { }

    #endregion
    
    #region Config

    protected T? CreateConfig<T>() where T : Config<T> {
        Config = ConfigManager.Instance.CreateConfig<T>(this);
        return (T?) Config;
    }

    #endregion
    
    #region Services

    protected void RegisterService<T>() where T : Service {
        ServiceManager.Instance.RegisterService<T>();
    }

    protected void UnregisterService<T>() where T : Service {
        ServiceManager.Instance.UnregisterService<T>();
    }
    
    protected void AcquireService<T>() where T : Service {
        ServiceManager.Instance.AcquireService<T>();
    }

    #endregion

    #region UI

    protected void RegisterModifierView<T>() where T : ModifierView {
        UIManager.Instance.RegisterModifierView<T>();
    }
    
    protected void UnregisterModifierView<T>() where T : ModifierView {
        UIManager.Instance.UnregisterModifierView<T>();
    }

    protected void RegisterView<T>() where T : View {
        
        if (ViewType != null || FlowControllerType != null) {
            Logger.Warn("Cannot register because a View or FlowController is already registered");
            return;
        }
        
        ViewType = typeof(T);
        
    }

    protected void UnregisterView() {
        ViewType = null;
    }

    protected void RegisterFlowController<T>() where T : FlowController {
        
        if (FlowControllerType != null || ViewType != null) {
            Logger.Warn("Cannot register because a FlowController or FlowController is already registered");
            return;
        }

        FlowControllerType = typeof(T);

    }

    protected void UnregisterFlowController() {
        FlowControllerType = null;
    }
    
    #endregion

    #region Harmony

    protected void Patch() {
        _harmony ??= new Harmony(Manifest.Id);
        _harmony.PatchAll(GetType().Assembly);
    }
    
    protected void Unpatch() {
        _harmony?.UnpatchSelf();
    }
    
    #endregion

    #region Zenject

    protected void AddInstaller<T>(Location location) where T : Zenject.Installer {
        ZenjectManager.Instance.AddInstaller<T>(location);
    }
    
    protected void RemoveInstaller<T>() where T : Zenject.Installer {
        ZenjectManager.Instance.RemoveInstaller<T>();
    }
    
    protected void Expose<T>(string contractName = "Environment") {
        ZenjectManager.Instance.Expose<T>(contractName);
    }
    
    protected void Unexpose<T>(string contractName = "Environment") {
        ZenjectManager.Instance.Unexpose<T>(contractName);
    }

    #endregion

}