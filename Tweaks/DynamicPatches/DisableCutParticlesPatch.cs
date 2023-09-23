using System.Reflection;

using BetterBeatSaber.Core.Harmomy.Dynamic;

namespace BetterBeatSaber.Tweaks.DynamicPatches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

public sealed class DisableCutParticlesPatch : DynamicPatch {

    public DisableCutParticlesPatch(bool enabled) : base(enabled) { }

    public static bool Prefix() => false;

    protected override MethodBase[] TargetMethods => new MethodBase[] {
        typeof(NoteCutParticlesEffect).GetMethod(nameof(NoteCutParticlesEffect.SpawnParticles), DefaultBindingFlags),
        typeof(NoteCutParticlesEffect).GetMethod("Awake", DefaultBindingFlags)
    };

}