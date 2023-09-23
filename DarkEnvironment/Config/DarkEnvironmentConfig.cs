using BetterBeatSaber.Core.Config;
using BetterBeatSaber.DarkEnvironment.DynamicPatches;

namespace BetterBeatSaber.DarkEnvironment.Config; 

public sealed class DarkEnvironmentConfig : Config<DarkEnvironmentConfig> {

    public bool HideLevelEnvironment { get; set; } = true;

    public bool HideMenuEnvironment { get; set; } = true;

    public bool DisableMenuCameraNoise { get; set; } = true;

    public BeatmapDataInsertBeatmapEventDataPatch BeatmapDataInsertBeatmapEventDataPatch { get; set; } = new(true);
    public LightSwitchEventEffectStartPatch LightSwitchEventEffectStartPatch { get; set; } = new(true);
    public SpectrogramUpdatePatch SpectrogramUpdatePatch { get; set; } = new(true);
    public StaticEnvironmentLightsAwakePatch StaticEnvironmentLightsAwakePatch { get; set; } = new(true);

}