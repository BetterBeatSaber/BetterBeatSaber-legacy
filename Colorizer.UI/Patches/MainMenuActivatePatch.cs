using BetterBeatSaber.Colorizer.UI.Colorizer;
using BetterBeatSaber.Colorizer.UI.Config;

using HarmonyLib;

using UnityEngine.UI;

namespace BetterBeatSaber.Colorizer.UI.Patches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

[HarmonyPatch(typeof(MainMenuViewController), "DidActivate")]
public static class MainMenuActivatePatch {

    [HarmonyPostfix]
    public static void Postfix(
        ref Button ____soloButton,
        ref Button ____partyButton,
        ref Button ____campaignButton,
        ref Button ____quitButton,
        ref Button ____howToPlayButton,
        ref Button ____beatmapEditorButton,
        ref Button ____multiplayerButton,
        ref Button ____optionsButton, 
        ref Button ____musicPackPromoBanner
    ) {

        if (!UIColorizerConfig.Instance.ColorizeMenuButtons) {
            ____soloButton.gameObject.AddComponent<UIImageColorizer.ImageViewColorizer>();
            ____partyButton.gameObject.AddComponent<UIImageColorizer.ImageViewColorizer>();
            ____campaignButton.gameObject.AddComponent<UIImageColorizer.ImageViewColorizer>();
            ____quitButton.gameObject.AddComponent<UIImageColorizer.ImageViewColorizer>();
            ____howToPlayButton.gameObject.AddComponent<UIImageColorizer.ImageViewColorizer>();
            ____beatmapEditorButton.gameObject.AddComponent<UIImageColorizer.ImageViewColorizer>();
            ____multiplayerButton.gameObject.AddComponent<UIImageColorizer.ImageViewColorizer>();
            ____optionsButton.gameObject.AddComponent<UIImageColorizer.ImageViewColorizer>();
        }
        
        // TODO: maybe move to Tweaks ?
        ____beatmapEditorButton.gameObject.SetActive(!UIColorizerConfig.Instance.HideEditorButton);
        ____musicPackPromoBanner.gameObject.SetActive(!UIColorizerConfig.Instance.HidePromotionButton);
        
    }

}