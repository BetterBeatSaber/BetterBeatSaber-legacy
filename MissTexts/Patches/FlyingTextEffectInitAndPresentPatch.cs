using System.Collections.Generic;

using HarmonyLib;

using TMPro;

using UnityEngine;

namespace BetterBeatSaber.MissTexts.Patches; 

[HarmonyPatch(typeof(FlyingTextEffect), "InitAndPresent", MethodType.Normal)]
public sealed class FlyingTextEffectInitAndPresentPatch {

    private static readonly List<string> MissTexts = new() {
        "FUCK",
        "SHIT",
        "BASTARD",
        "TRASH",
        "NOOB",
        "IDIOT",
        "HAHA",
        "TRACKING?",
        "LAG?",
        "OOF",
        "MISS",
        "WER DAS LIEST IST DUMM",
        "LACKAFFE",
        "!bsr 15357",
        "SHAME",
        "ASCHE AUF DEIN HAUPT",
        "NAILIK HASST DICH"
    };
    
    private static readonly RandomObjectPicker<string> RandomMissTextPicker = new(MissTexts.ToArray(), .01f);
    
    // ReSharper disable once InconsistentNaming
    [HarmonyPrefix]
    public static bool Prefix(ref string text, ref TextMeshPro ____text) {
        
        if (text != "MISS")
            return true;

        var textMeshPro = ____text;

        text = RandomMissTextPicker.PickRandomObject();

        textMeshPro.color = Color.red;
        textMeshPro.overflowMode = TextOverflowModes.Overflow;
        textMeshPro.enableWordWrapping = false;
        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        textMeshPro.fontStyle = FontStyles.Italic | FontStyles.Bold;

        return true;
        
    }

}