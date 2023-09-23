using BetterBeatSaber.Tweaks.Config;

using HarmonyLib;

using UnityEngine;

namespace BetterBeatSaber.Tweaks.Patches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

[HarmonyPatch(typeof(GameNoteController), nameof(GameNoteController.Init))]
public static class GameNoteControllerInitPatch {
    
    public static void Postfix(ref GameNoteController __instance) {
        __instance.gameObject.transform.localScale = TweaksConfig.Instance.NoteSize * Vector3.one;
    }

}