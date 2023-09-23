using System.Reflection;

using BetterBeatSaber.Core.Harmomy;
using BetterBeatSaber.Core.Harmomy.Dynamic;
using BetterBeatSaber.DarkEnvironment.Config;

namespace BetterBeatSaber.DarkEnvironment.DynamicPatches; 

public sealed class LightSwitchEventEffectStartPatch : DynamicPatch {

    public LightSwitchEventEffectStartPatch(bool enabled) : base(enabled) { }

    protected override MethodBase TargetMethod => typeof(LightSwitchEventEffect).GetMethod("Start")!;
    
    public static bool Prefix() {
        return !DarkEnvironmentConfig.Instance.HideLevelEnvironment;
    }
    
}