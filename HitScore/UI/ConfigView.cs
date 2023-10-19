using BetterBeatSaber.Core.UI;
using BetterBeatSaber.HitScore.Config;

namespace BetterBeatSaber.HitScore.UI;

// ReSharper disable UnusedMember.Global
public sealed class ConfigView : View<ConfigView> {

    public bool Enable {
        get => HitScoreConfig.Instance.Enable;
        set {
            HitScoreConfig.Instance.Enable = value;
            HitScoreConfig.Instance.Updated = true;
        }
    }
    
    public bool EnableGlow {
        get => HitScoreConfig.Instance.EnableGlow;
        set {
            HitScoreConfig.Instance.EnableGlow = value;
            HitScoreConfig.Instance.Updated = true;
        }
    }
    
    public float Scale {
        get => HitScoreConfig.Instance.Scale * 100;
        set {
            HitScoreConfig.Instance.Scale = value / 100;
            HitScoreConfig.Instance.Updated = true;
        }
    }
    
}