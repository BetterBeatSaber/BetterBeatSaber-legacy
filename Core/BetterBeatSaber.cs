using System;
using System.Linq;

using BeatSaberMarkupLanguage;

using BetterBeatSaber.Core.Bindings;
using BetterBeatSaber.Core.Config;
using BetterBeatSaber.Core.Game;
using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Core.Manager.Audio;
using BetterBeatSaber.Core.Manager.Interop;
using BetterBeatSaber.Core.Network;
using BetterBeatSaber.Core.Patches;
using BetterBeatSaber.Core.Provider;
using BetterBeatSaber.Core.UI;
using BetterBeatSaber.Core.UI.BSML;
using BetterBeatSaber.Core.UI.Components;
using BetterBeatSaber.Core.UI.Main;
using BetterBeatSaber.Core.Utilities;
using BetterBeatSaber.Core.Zenject;

using HarmonyLib;

using UnityEngine;

using Location = BetterBeatSaber.Core.Zenject.Location;
using Logger = IPA.Logging.Logger;
using Version = Hive.Versioning.Version;

namespace BetterBeatSaber.Core;

// ReSharper disable UnusedMember.Global

public sealed class BetterBeatSaber : Singleton<BetterBeatSaber> {

    public static readonly Version Version = new("2.1.2-dev.2");
    
    public static BetterLogger Logger { get; private set; } = null!;

    internal static readonly Harmony Harmony = new("betterbs.xyz");

    public BetterBeatSaber(Logger logger) {
        Logger = new BetterLogger(logger);
    }
    
    #region Init, Start & Exit

    public void Init() {

        ZenjectManager.Instance.OnInstall += (location, container) => {
            
            if(location.IsPlayer() || location.HasFlag(Location.Menu))
                container.Bind<AssetProvider>().AsSingle().NonLazy();

            if (!location.HasFlag(Location.Menu))
                return;

            if(CoreConfig.Instance.CustomTitle)
                container.Bind<MenuSignBinding>().FromNewComponentOn(new GameObject(nameof(MenuSignBinding))).AsSingle().NonLazy();
            
            // new year day, beat saber release and my bd lol
            if (DateTime.Now is { Day: 1, Month: 1 } or { Day: 1, Month: 5 } or { Day: 8, Month: 10 })
                container.BindInterfacesAndSelfTo<FireworksBinding>().AsSingle();

        };
        
        ThreadDispatcher.Instance.Init();
        
        Harmony.PatchAll();

    }

    public void Start() {

        RGB.Instantiate();

        MenuButtonsViewControllerDidActivatePatch.Patch(Harmony);
        
        BSMLParser.instance.RegisterTag(new RGBTextTag());
        BSMLParser.instance.RegisterTag(new AvatarComponent.Tag());
        
        #if DEBUG
        if(Environment.GetCommandLineArgs().Contains("--explorer"))
            UnityExplorer.ExplorerStandalone.CreateInstance();
        #endif

        BeatSaber.Instantiate();
        
        #region Manager

        AuthManager.Instance.Init();
        
        ConfigManager.Instance.Init();
        
        ConfigManager.Instance.CreateConfig<CoreConfig>("core");
        
        NetworkClient.Instantiate();
        
        ZenjectManager.Instance.Init();
        AssetManager.Instance.Init();
        UIManager.Instance.Init();
        PlayerManager.Instance.Init();
        ModuleManager.Instance.Init();
        FriendManager.Instance.Init();
        MapManager.Instance.Init();
        InteropManager.Instance.Init();
        BackupManager.Instance.Init();

        AudioManager.Instantiate();
        PresenceManager.Instantiate();

        #endregion

        #region Exposures

        ZenjectManager.Instance.Expose<GameplayCoreSceneSetupData>();
        ZenjectManager.Instance.Expose<AudioTimeSyncController>();
        ZenjectManager.Instance.Expose<PauseController>();
        ZenjectManager.Instance.Expose<FireworksController>();
        
        #endregion
        
    }

    public void Exit() {
        CoreConfig.Instance.Save();
    }
    
    #endregion

    #region Enable & Disable

    public void Enable() {
        
        ModuleManager.Instance.Enable();
        
        MainFlowController.Initialize();
        
    }

    public void Disable() {
    }

    #endregion

}