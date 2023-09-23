using System.Reflection;

using BetterBeatSaber.Core.Harmomy;
using BetterBeatSaber.Core.Harmomy.Dynamic;
using BetterBeatSaber.DarkEnvironment.Config;

namespace BetterBeatSaber.DarkEnvironment.DynamicPatches; 

public sealed class BeatmapDataInsertBeatmapEventDataPatch : DynamicPatch {

    public BeatmapDataInsertBeatmapEventDataPatch(bool enabled) : base(enabled) { }

    protected override MethodBase TargetMethod => typeof(BeatmapData).GetMethod("InsertBeatmapEventData")!;

    public static bool Prefix() {
        return !DarkEnvironmentConfig.Instance.HideLevelEnvironment;
    }
    
}