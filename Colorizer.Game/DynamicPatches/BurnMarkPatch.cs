using BetterBeatSaber.Colorizer.Game.Colorizer;
using BetterBeatSaber.Core.Harmomy.Dynamic;

using IPA.Utilities;

namespace BetterBeatSaber.Colorizer.Game.DynamicPatches;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

[DynamicPatch(typeof(SaberBurnMarkArea), "Start")]
public sealed class BurnMarkPatch : DynamicPatch {

    public BurnMarkPatch(bool enabled) : base(enabled) { }

    public static void Postfix(SaberBurnMarkArea __instance) {
        
        __instance.enabled = true;
        
        if(!__instance.gameObject.activeInHierarchy)
            __instance.gameObject.SetActive(true);
        
        __instance.SetField("_burnMarksFadeOutStrength", .5f);

        if (__instance.GetComponent<BurnMarksColorizer>() != null)
            return;
        
        var burnMarksColorizer = __instance.gameObject.AddComponent<BurnMarksColorizer>();
        burnMarksColorizer.saberBurnMarkArea = __instance;
        
    }
    
}