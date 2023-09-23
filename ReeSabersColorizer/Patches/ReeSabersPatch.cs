using BetterBeatSaber.Core.Utilities;

using HarmonyLib;

using IPA.Utilities;

using ReeSabers;

namespace BetterBeatSaber.ReeSabersColorizer.Patches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

[HarmonyPatch(typeof(ColorController), "Update")]
public sealed class ReeSabersPatch {
    
    [HarmonyPostfix]
    public static void Postfix(ColorController __instance) {
        __instance.HsbTransform.SetValue(new HsbTransform(RGB.Hue0, 1f, 0f, ColorTransformType.HueOverride), __instance);
        __instance.SetField("_isDirty", true);
    }

}