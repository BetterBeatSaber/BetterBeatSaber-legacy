using BetterBeatSaber.Core.Harmomy.Dynamic;

namespace BetterBeatSaber.Tweaks.DynamicPatches; 

[DynamicPatch(typeof(MissedNoteEffectSpawner), nameof(MissedNoteEffectSpawner.HandleNoteWasMissed))]
public sealed class DisableMissTextPatch : DynamicPatch {

    public DisableMissTextPatch(bool enabled) : base(enabled) { }

    private static void Prefix() {
        
    }
    
}