using BetterBeatSaber.Core.Harmomy.Dynamic;

namespace BetterBeatSaber.Tweaks.DynamicPatches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

[DynamicPatch(typeof(BombExplosionEffect), nameof(BombExplosionEffect.SpawnExplosion))]
public sealed class DisableBombExplosionEffectPatch : DynamicPatch {

    public DisableBombExplosionEffectPatch(bool enabled) : base(enabled) { }
    
    public static bool Prefix() => false;

}