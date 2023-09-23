using BeatSaberMarkupLanguage.Tags;

using BetterBeatSaber.Colorizer.UI.Colorizer;
using BetterBeatSaber.Colorizer.UI.Config;

using HarmonyLib;

using UnityEngine;

namespace BetterBeatSaber.Colorizer.UI.Patches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

[HarmonyPatch(typeof(ButtonTag), nameof(ButtonTag.CreateObject))]
public static class ButtonTagPatch {

    [HarmonyPostfix]
    private static void Postfix(GameObject __result) {
        if (!UIColorizerConfig.Instance.ColorizeButtons)
            return;
        __result.AddComponent<UIImageColorizer.ImageViewColorizer>();
    }

}