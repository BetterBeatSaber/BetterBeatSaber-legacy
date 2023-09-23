using BeatSaberMarkupLanguage.Attributes;

using BetterBeatSaber.Core.UI;
using BetterBeatSaber.Tweaks.Config;

using UnityEngine;

namespace BetterBeatSaber.Tweaks.UI;

#if DEBUG
[HotReload(RelativePathToLayout = "./ConfigView.bsml")]
#endif
public sealed class ConfigView : View<ConfigView> {

    public float NoteSize {
        get => TweaksConfig.Instance.NoteSize * 100f;
        set {
            TweaksConfig.Instance.NoteSize = value / 100f;
            TweaksConfig.Instance.Updated = true;
        }
    }
    
    public float BombSize {
        get => TweaksConfig.Instance.BombSize * 100;
        set {
            TweaksConfig.Instance.BombSize = value / 100;
            TweaksConfig.Instance.Updated = true;
        }
    }
    
    public bool DisableBombExplosionEffect {
        get => TweaksConfig.Instance.DisableBombExplosionEffect.Enabled;
        set {
            TweaksConfig.Instance.DisableBombExplosionEffect.Enabled = value;
            TweaksConfig.Instance.Updated = true;
        }
    }
    
    public Color BombColor {
        get => TweaksConfig.Instance.BombColor;
        set {
            TweaksConfig.Instance.BombColor = value;
            TweaksConfig.Instance.Updated = true;
        }
    }

    public bool DisableComboBreakEffect {
        get => TweaksConfig.Instance.DisableComboBreakEffect.Enabled;
        set {
            TweaksConfig.Instance.DisableComboBreakEffect.Enabled = value;
            TweaksConfig.Instance.Updated = true;
        }
    }
    
    public bool DisableCutParticles {
        get => TweaksConfig.Instance.DisableCutParticles.Enabled;
        set {
            TweaksConfig.Instance.DisableCutParticles.Enabled = value;
            TweaksConfig.Instance.Updated = true;
        }
    }
 
    public bool DisableAprilFoolsAndEarthDayStuff {
        get => TweaksConfig.Instance.DisableAprilFoolsAndEarthDayStuff.Enabled;
        set {
            TweaksConfig.Instance.DisableAprilFoolsAndEarthDayStuff.Enabled = value;
            TweaksConfig.Instance.Updated = true;
        }
    }
        
}