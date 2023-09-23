using BetterBeatSaber.Colorizer.Game.Utilities;

using HarmonyLib;

using UnityEngine;

namespace BetterBeatSaber.Colorizer.Game.Patches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

[HarmonyPatch(typeof(BaseNoteVisuals), "OnDestroy")]
public static class BaseNoteVisualsOnDestroyPatch {

    [HarmonyPostfix]
    [HarmonyPriority(int.MaxValue)]
    public static void Postfix(NoteController ____noteController) {
        var outline = ____noteController.gameObject.GetComponent<Outline>();
        if (outline != null)
            Object.Destroy(outline);
    }

}