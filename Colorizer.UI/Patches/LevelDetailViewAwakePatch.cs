using BetterBeatSaber.Colorizer.UI.Colorizer;
using BetterBeatSaber.Colorizer.UI.Config;

using HarmonyLib;

using UnityEngine.UI;

namespace BetterBeatSaber.Colorizer.UI.Patches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

// ReSharper disable SuggestBaseTypeForParameter

[HarmonyPatch(typeof(StandardLevelDetailView), "Awake")]
public static class LevelDetailViewAwakePatch {

    [HarmonyPostfix]
    private static void Postfix(
        ref Button ____actionButton,
        ref Button ____practiceButton
    ) {
        
        if (!UIColorizerConfig.Instance.ColorizeButtons)
            return;
        
        ____actionButton.gameObject.AddComponent<UIImageColorizer.ImageColorizer>();
        ____practiceButton.gameObject.AddComponent<UIImageColorizer.ImageViewColorizer>();
        
    }

}