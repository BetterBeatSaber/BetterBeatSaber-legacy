using BetterBeatSaber.Core.Config;
using BetterBeatSaber.Tweaks.DynamicPatches;
using BetterBeatSaber.Tweaks.Patches;

using UnityEngine;

namespace BetterBeatSaber.Tweaks.Config; 

public sealed class TweaksConfig : Config<TweaksConfig> {

    public float NoteSize { get; set; } = 1f;
    public float BombSize { get; set; } = 1f;

    public Color BombColor { get; set; } = BombPatch.DefaultColor;

    public DisableComboBreakEffectPatch DisableComboBreakEffect { get; set; } = new(true);
    public DisableBombExplosionEffectPatch DisableBombExplosionEffect { get; set; } = new(true);
    public DisableCutParticlesPatch DisableCutParticles { get; set; } = new(true);
    public DisableAprilFoolsAndEarthDayPatch DisableAprilFoolsAndEarthDayStuff { get; set; } = new(true);

}