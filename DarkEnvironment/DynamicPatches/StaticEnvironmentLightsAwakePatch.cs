using System.Reflection;

using BetterBeatSaber.Core.Harmomy;
using BetterBeatSaber.Core.Harmomy.Dynamic;
using BetterBeatSaber.DarkEnvironment.Config;

namespace BetterBeatSaber.DarkEnvironment.DynamicPatches; 

public sealed class StaticEnvironmentLightsAwakePatch : DynamicPatch {

    public StaticEnvironmentLightsAwakePatch(bool enabled) : base(enabled) { }

    protected override MethodBase TargetMethod => typeof(StaticEnvironmentLights).GetMethod("Awake")!;

    public static bool Prefix() {
        return !DarkEnvironmentConfig.Instance.HideLevelEnvironment;
    }
    
}