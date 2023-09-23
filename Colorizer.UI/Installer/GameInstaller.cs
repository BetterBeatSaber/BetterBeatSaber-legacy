using BetterBeatSaber.Colorizer.UI.HudModifier;

using UnityEngine;

namespace BetterBeatSaber.Colorizer.UI.Installer; 

public sealed class GameInstaller : Core.Zenject.Installer {

    public override void Install() {
        
        BindHudModifier<ComboHudModifier>();
        BindHudModifier<EnergyHudModifier>();
        BindHudModifier<MultiplierHudModifier>();
        BindHudModifier<ProgressHudModifier>();
        BindHudModifier<RemoveBackgroundHudModifier>();
        BindHudModifier<ScoreHudModifier>();

    }

    public override void Uninstall() {
    }

    private void BindHudModifier<T>() where T : HudModifier.HudModifier {
        Container.Bind<T>().FromNewComponentOn(new GameObject(typeof(T).Name)).AsSingle().NonLazy();
    }
    
}