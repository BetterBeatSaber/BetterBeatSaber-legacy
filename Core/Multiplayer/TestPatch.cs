using BeatSaberMarkupLanguage;

using HarmonyLib;

using UnityEngine.UI;

namespace BetterBeatSaber.Core.Multiplayer; 

//[HarmonyPatch(typeof(StandardLevelDetailView), nameof(StandardLevelDetailView.SetContent))]
public class TestPatch {

    //[HarmonyPriority(int.MaxValue)]
    //[HarmonyPrefix]
    public static void Prefix(
        // ReSharper disable once InconsistentNaming
        Button ____actionButton,
        // ReSharper disable once InconsistentNaming
        Button ____practiceButton
        ) {
        // check if is in lobby etc...
        ____actionButton.SetButtonText("Select");
        ____actionButton.onClick = null;
        ____practiceButton.gameObject.SetActive(false);
    }

}