using BetterBeatSaber.Core.Harmomy.Dynamic;

using UnityEngine;

namespace BetterBeatSaber.Tweaks.DynamicPatches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

[DynamicPatch(typeof(ComboUIController), "Start")]
public sealed class DisableComboBreakEffectPatch : DynamicPatch {
    
    public DisableComboBreakEffectPatch(bool enabled) : base(enabled) { }

    public static void Postfix(Animator ____animator) => ____animator.speed = 69420f;

}