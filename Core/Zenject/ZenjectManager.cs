using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Core.Patches;
using BetterBeatSaber.Core.Zenject.Internal;

using HarmonyLib;

using IPA.Utilities;

using UnityEngine;

using Zenject;

namespace BetterBeatSaber.Core.Zenject;

public sealed class ZenjectManager : Manager<ZenjectManager> {

    private static readonly Dictionary<Type, Location> TypesToPatch = new() {
        { typeof(PCAppInit), Location.App },
        { typeof(MainSettingsMenuViewControllersInstaller), Location.Menu },
        { typeof(StandardGameplayInstaller), Location.StandardPlayer },
        { typeof(MissionGameplayInstaller), Location.CampaignPlayer },
        { typeof(MultiplayerLocalActivePlayerInstaller), Location.MultiPlayer },
        { typeof(TutorialInstaller), Location.Tutorial },
        { typeof(GameCoreSceneSetup), Location.GameCore },
        { typeof(MultiplayerCoreInstaller), Location.MultiplayerCore },
        { typeof(MultiplayerConnectedPlayerInstaller), Location.ConnectedPlayer },
        { typeof(MultiplayerLocalPlayerInstaller), Location.AlwaysMultiPlayer },
        { typeof(MultiplayerLocalInactivePlayerInstaller), Location.InactiveMultiPlayer }
    };
    
    public event Action<Location, DiContainer>? OnInstall;

    private readonly List<InstallData> _installData = new();
    private readonly List<ExposeData> _exposeData = new();

    #region Init & Exit

    public override void Init() {
        ContextInstallInstallersPatch.Install += Install;
        foreach (var type in TypesToPatch.Keys) {
            BetterBeatSaber.Harmony.Patch(type.GetMethod("InstallBindings")!, postfix: new HarmonyMethod(typeof(ZenjectManager).GetMethod(nameof(InstallBindings), BindingFlags.Static | BindingFlags.NonPublic)));
        }
    }

    public override void Exit() {
        foreach (var type in TypesToPatch.Keys) {
            BetterBeatSaber.Harmony.Unpatch(type.GetMethod("InstallBindings")!, HarmonyPatchType.Postfix);
        }
        ContextInstallInstallersPatch.Install -= Install;
        // TODO: Uninstall alllll
        // TODO: Unexpose alllll
        _installData.Clear();
    }

    #endregion

    public void AddInstaller<T>(Location location) where T : Installer {
        AddInstaller(typeof(T), location);
    }

    public void AddInstaller(Type type, Location location) {
        _installData.Add(new InstallData(type, location));
    }

    public void RemoveInstaller<T>() where T : Installer {
        RemoveInstaller(typeof(T));
    }

    public void RemoveInstaller(Type type) {
        _installData.RemoveAll(installData => installData.Type == type);
    }

    public void RemoveInstallers(Assembly? assembly = null) {
        foreach (var installData in _installData.Where(installData => installData.Type.Assembly == assembly)) {
            RemoveInstaller(installData.Type);
        }
    }

    public void Expose<T>(string contractName = "Environment") {
        Expose(typeof(T), contractName);
    }

    public void Expose(Type type, string contractName = "Environment") {
        _exposeData.Add(new ExposeData(type, contractName));
    }

    public void Unexpose<T>(string contractName = "Environment") {
        Unexpose(typeof(T), contractName);
    }

    public void Unexpose(Type type, string contractName = "Environment") {
        _exposeData.RemoveAll(exposeData => exposeData.Type == type && exposeData.ContractName == contractName);
    }

    #region Private

    // ReSharper disable once InconsistentNaming
    private static void InstallBindings(object __instance) {
        
        var container = (DiContainer?) typeof(MonoInstallerBase).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)[0].GetValue(__instance);
        if (container == null)
            return;

        var type = __instance.GetType();
        
        foreach (var installData in GetInstallDataFromType(type)) {
            try {
                
                installData.Type.Construct<Installer>(new Dictionary<string, object> {
                    { nameof(Installer.Container), container }
                })?.Install();
                
            } catch (Exception exception) {
                Console.WriteLine("Failed to install " + installData.Type + " " + exception);
            }
        }
        
        if(TypesToPatch.TryGetValue(type, out var location))
            Instance.OnInstall?.Invoke(location, container);
        
    }
    
    private static readonly FieldAccessor<SceneDecoratorContext, List<MonoBehaviour>>.Accessor SceneDecoratorInjectables = FieldAccessor<SceneDecoratorContext, List<MonoBehaviour>>.GetAccessor("_injectableMonoBehaviours");
    
    public static bool InitialSceneConstructionRegistered { get; private set; }
    private void Install(Context context) {
        
        if (context.name == "AppCoreSceneContext")
            InitialSceneConstructionRegistered = true;

        if (!InitialSceneConstructionRegistered)
            return;

        if (context is not SceneDecoratorContext sceneContext)
            return;
        
        foreach (var exposeData in _exposeData) {
            
            if (exposeData.ContractName != sceneContext.DecoratedContractName || string.IsNullOrEmpty(sceneContext.DecoratedContractName) || string.IsNullOrWhiteSpace(exposeData.ContractName))
                continue;
            
            var toExpose = SceneDecoratorInjectables(ref sceneContext).FirstOrDefault(d => d.GetType() == exposeData.Type);
            if (toExpose != null && !sceneContext.Container.HasBinding(exposeData.Type)) {
                sceneContext.Container.Bind(exposeData.Type).FromInstance(toExpose).AsSingle();
            } else {
                //Console.WriteLine($"Could not expose {exposeData.Type.Name} because {(toExpose == null ? "it could not be found in the SceneContextDecorator" : "it is already exposed")}");
            }
            
        }

    }

    private static IEnumerable<InstallData> GetInstallDataFromType(Type type) {
        return Instance._installData.Where(installData => TypesToPatch.ContainsKey(type) && installData.Location.HasFlag(TypesToPatch[type]));
    }

    #endregion
    
}