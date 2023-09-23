using BetterBeatSaber.Core;
using BetterBeatSaber.HitScore.Config;
using BetterBeatSaber.HitScore.UI;

namespace BetterBeatSaber.HitScore;

// ReSharper disable once UnusedType.Global
public sealed class HitScore : Module {

    public override void Init() {
        CreateConfig<HitScoreConfig>();
    }

    public override void Enable() {
        
        RegisterView<ConfigView>();
        
        Patch();
        
    }

    public override void Disable() {
        
        UnregisterView();
        
        Unpatch();
        
    }

}