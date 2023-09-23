using BeatSaberMarkupLanguage.Attributes;

using BetterBeatSaber.Core.UI;
using BetterBeatSaber.DarkEnvironment.Config;
using BetterBeatSaber.DarkEnvironment.Utilities;

namespace BetterBeatSaber.DarkEnvironment.UI; 

public sealed class DarkEnvironmentModifierView : ModifierView {

    public override string Title => "Dark Environment";
    
    [UIValue("HideLevelEnvironment")]
    public bool HideLevelEnvironment {
        get => DarkEnvironmentConfig.Instance.HideLevelEnvironment;
        set {
            DarkEnvironmentConfig.Instance.BeatmapDataInsertBeatmapEventDataPatch.Enabled = value;
            DarkEnvironmentConfig.Instance.LightSwitchEventEffectStartPatch.Enabled = value;
            DarkEnvironmentConfig.Instance.SpectrogramUpdatePatch.Enabled = value;
            DarkEnvironmentConfig.Instance.StaticEnvironmentLightsAwakePatch.Enabled = value;
            DarkEnvironmentConfig.Instance.HideLevelEnvironment = value;
            DarkEnvironmentConfig.Instance.Save();
        }
    }
    
    [UIValue("HideMenuEnvironment")]
    public bool HideMenuEnvironment {
        get => DarkEnvironmentConfig.Instance.HideMenuEnvironment;
        set {
            DarkEnvironmentConfig.Instance.HideMenuEnvironment = value;
            DarkEnvironmentConfig.Instance.Save();
            EnvironmentHider.SetMenuEnvironment(!value);
        }
    }

}