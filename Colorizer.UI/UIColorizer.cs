using BetterBeatSaber.Colorizer.UI.Config;
using BetterBeatSaber.Colorizer.UI.Installer;
using BetterBeatSaber.Colorizer.UI.UI;
using BetterBeatSaber.Core;
using BetterBeatSaber.Core.Zenject;

namespace BetterBeatSaber.Colorizer.UI;

// ReSharper disable once UnusedType.Global
public sealed class UIColorizer : Module {

    public override void Init() {
        CreateConfig<UIColorizerConfig>();
    }

    public override void Exit() {
    }

    public override void Enable() {
        
        RegisterView<ConfigView>();
        
        Expose<PlayerSaveData.GameplayModifiers>();
        Expose<ImmediateRankUIPanel>();
        Expose<ScoreMultiplierUIController>();
        Expose<SongProgressUIController>();
        Expose<GameEnergyUIPanel>();
        Expose<ComboUIController>();
        
        AddInstaller<GameInstaller>(Location.StandardPlayer | Location.CampaignPlayer);

        Patch();
        
    }

    public override void Disable() {
        
        Unexpose<PlayerSaveData.GameplayModifiers>();
        Unexpose<ImmediateRankUIPanel>();
        Unexpose<ScoreMultiplierUIController>();
        Unexpose<SongProgressUIController>();
        Unexpose<GameEnergyUIPanel>();
        Unexpose<ComboUIController>();
        
        RemoveInstaller<GameInstaller>();

        UnregisterView();
        
        Unpatch();
        
    }

}