using BetterBeatSaber.Colorizer.UI.Colorizer;
using BetterBeatSaber.Colorizer.UI.Config;

using HarmonyLib;

using UnityEngine.UI;

namespace BetterBeatSaber.Colorizer.UI.Patches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

[HarmonyPatch(typeof(PracticeViewController), "DidActivate")]
public static class PracticeViewDidActivatePatch {

    [HarmonyPostfix]
    private static void Postfix(ref Button ____playButton) {
        if (!UIColorizerConfig.Instance.ColorizeButtons)
            return;
        ____playButton.gameObject.AddComponent<UIImageColorizer.ImageColorizer>();
    }

}