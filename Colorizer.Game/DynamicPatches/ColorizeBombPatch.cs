using System.Reflection;

using BetterBeatSaber.Colorizer.Game.Colorizer;
using BetterBeatSaber.Core.Harmomy.Dynamic;

namespace BetterBeatSaber.Colorizer.Game.DynamicPatches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

[DynamicPatch(typeof(BombNoteController), nameof(BombNoteController.Init), BindingFlags.Public | BindingFlags.Instance)]
public sealed class ColorizeBombPatch : DynamicPatch {

    public ColorizeBombPatch(bool enabled) : base(enabled) { }

    //private static readonly Color DefaultColor = new(.251f, .251f, .251f, 1f);

    private static void Postfix(ref BombNoteController __instance) {
        if (!__instance.gameObject.TryGetComponent(typeof(BombColorizer), out _))
            __instance.gameObject.AddComponent<BombColorizer>();
    }
    
}