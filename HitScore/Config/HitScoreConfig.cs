using BetterBeatSaber.Core.Config;

namespace BetterBeatSaber.HitScore.Config; 

// ReSharper disable UnusedAutoPropertyAccessor.Global

public partial class HitScoreConfig : Config<HitScoreConfig> {

    public bool Enable { get; set; } = true;
    public bool EnableGlow { get; set; }
    public float Scale { get; set; } = 1f;

}