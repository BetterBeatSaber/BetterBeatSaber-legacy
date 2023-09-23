using System;
using System.Linq;
using System.Reflection;

using BetterBeatSaber.Core.UI.BSML;

using HarmonyLib;

using IPA.Loader;

using TMPro;

using UnityEngine;

namespace BetterBeatSaber.Core.Patches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

internal static class MenuButtonsViewControllerDidActivatePatch {

    public static Type MenuButtonsViewControllerType => PluginManager.GetPlugin("BeatSaberMarkupLanguage").Assembly.GetType("BeatSaberMarkupLanguage.MenuButtons.MenuButtonsViewController");
    
    internal static void Patch(Harmony harmony) {
        harmony.Patch(MenuButtonsViewControllerType.GetMethod("DidActivate", BindingFlags.Instance | BindingFlags.NonPublic), null, new HarmonyMethod(typeof(MenuButtonsViewControllerDidActivatePatch).GetMethod(nameof(Postfix), BindingFlags.NonPublic | BindingFlags.Static)));
    }
    
    private static void Postfix() {
        
        var menuButtonsViewController = (MonoBehaviour?) Resources.FindObjectsOfTypeAll(MenuButtonsViewControllerType).FirstOrDefault();
        if (menuButtonsViewController == null)
            return;
        
        var buttonTexts = menuButtonsViewController.gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        if (buttonTexts == null || buttonTexts.Length == 0)
            return;

        var buttonText = buttonTexts.FirstOrDefault(text => text.text == "<b>Better Beat Saber</b>");
        if (buttonText == null)
            return;
        
        buttonText.gameObject.AddComponent<RGBTextTag.RGBTextTagColorizer>();
        
    }

}