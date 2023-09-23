using System.Reflection;

using BetterBeatSaber.Core.Harmomy;
using BetterBeatSaber.Core.Harmomy.Dynamic;
using BetterBeatSaber.DarkEnvironment.Config;

namespace BetterBeatSaber.DarkEnvironment.DynamicPatches; 

public sealed class SpectrogramUpdatePatch : DynamicPatch {

    public SpectrogramUpdatePatch(bool enabled) : base(enabled) { }

    protected override MethodBase TargetMethod => typeof(Spectrogram).GetMethod("Update")!;

    public static bool Prefix() {
        return !DarkEnvironmentConfig.Instance.HideLevelEnvironment;
    }
    
}