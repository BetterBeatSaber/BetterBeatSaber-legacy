using BetterBeatSaber.Colorizer.Game.Config;
using BetterBeatSaber.Core.Utilities;

using HarmonyLib;

using UnityEngine;

namespace BetterBeatSaber.Colorizer.Game.Patches; 

[HarmonyPatch(typeof(NoteDebris), nameof(NoteDebris.Init))]
[HarmonyPatch(typeof(NoteDebris), "Update")]
internal static class NoteDebrisInitAndUpdatePatch {

    // ReSharper disable once InconsistentNaming
    [HarmonyPostfix]
    [HarmonyPriority(int.MaxValue)]
    private static void Postfix(MaterialPropertyBlockController ____materialPropertyBlockController) {
        if (!GameColorizerConfig.Instance.Notes.ColorizeDebris)
            return;
        var materialPropertyBlock = ____materialPropertyBlockController.materialPropertyBlock;
        materialPropertyBlock.SetColor(Shader.PropertyToID("_Color"), RGB.Color0);
        ____materialPropertyBlockController.ApplyChanges();
    }

}