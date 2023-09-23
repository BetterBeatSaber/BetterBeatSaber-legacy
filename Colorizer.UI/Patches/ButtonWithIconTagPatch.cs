using BeatSaberMarkupLanguage.Tags;

using BetterBeatSaber.Colorizer.UI.Colorizer;
using BetterBeatSaber.Colorizer.UI.Config;

using HarmonyLib;

using UnityEngine;

namespace BetterBeatSaber.Colorizer.UI.Patches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

[HarmonyPatch(typeof(ButtonWithIconTag), nameof(ButtonWithIconTag.CreateObject))]
public static class ButtonWithIconTagPatch {

    [HarmonyPostfix]
    private static void Postfix(GameObject __result) {
        
        if (!UIColorizerConfig.Instance.ColorizeButtons)
            return;

        var colorizer = __result.GetComponent<UIImageColorizer.ImageViewColorizer>();
        if (colorizer != null) {
            colorizer.ReloadImageViews();
        } else {
            __result.AddComponent<UIImageColorizer.ImageViewColorizer>();
        }
        
    }

}