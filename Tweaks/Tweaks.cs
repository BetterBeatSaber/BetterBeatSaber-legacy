using BetterBeatSaber.Core;
using BetterBeatSaber.Tweaks.Config;
using BetterBeatSaber.Tweaks.UI;

namespace BetterBeatSaber.Tweaks;

// ReSharper disable once UnusedType.Global
public sealed class Tweaks : Module {

    public override void Init() {
        CreateConfig<TweaksConfig>();
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