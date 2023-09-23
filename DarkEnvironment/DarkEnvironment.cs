using BetterBeatSaber.Core;
using BetterBeatSaber.Core.Zenject;
using BetterBeatSaber.DarkEnvironment.Config;
using BetterBeatSaber.DarkEnvironment.UI;
using BetterBeatSaber.DarkEnvironment.Utilities;

using Zenject;

namespace BetterBeatSaber.DarkEnvironment; 

// ReSharper disable once UnusedType.Global
public sealed class DarkEnvironment : Module {

    public override void Init() {
        CreateConfig<DarkEnvironmentConfig>();
    }

    public override void Enable() {
        
        Patch();
        
        ZenjectManager.Instance.OnInstall += OnInstall;
        
        RegisterModifierView<DarkEnvironmentModifierView>();
        
    }

    public override void Disable() {
        
        UnregisterModifierView<DarkEnvironmentModifierView>();
        
        ZenjectManager.Instance.OnInstall -= OnInstall;
        
        Unpatch();
        
    }
    
    private static void OnInstall(Location location, DiContainer _) {
        if (location == Location.Menu) {
            EnvironmentHider.LoadMenuGameObjects();
            EnvironmentHider.SetMenuEnvironment(!DarkEnvironmentConfig.Instance.HideMenuEnvironment);
        } else if (location.IsPlayer()) {
            if (DarkEnvironmentConfig.Instance.HideLevelEnvironment) {
                EnvironmentHider.HideEnvironment();
            }
        }
    }

}