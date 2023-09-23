using System.Reflection;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Tweaks.Config;

using HarmonyLib;

using UnityEngine;

namespace BetterBeatSaber.Tweaks.Patches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

// ReSharper disable Unity.NoNullPropagation

[HarmonyPatch(typeof(BeatmapObjectsInstaller), "InstallBindings")]
public static class BombPatch {

    internal static readonly Color DefaultColor = Color.black.WithAlpha(0);

    private static readonly int _SimpleColor = Shader.PropertyToID("_SimpleColor");

    private static Color? FirstBombColor, SecondBombColor;
    
    private static FieldInfo? FirstBombMaterialField => typeof(ConditionalMaterialSwitcher).GetField("_material0", BindingFlags.Instance | BindingFlags.NonPublic);
    private static FieldInfo? SecondBombMaterialField => typeof(ConditionalMaterialSwitcher).GetField("_material1", BindingFlags.Instance | BindingFlags.NonPublic);

    [HarmonyPriority(int.MaxValue)]
    private static void Postfix(BombNoteController ____bombNotePrefab) {
        
        var conditionalMaterialSwitcher = ____bombNotePrefab.GetComponentInChildren<ConditionalMaterialSwitcher>();
        if(conditionalMaterialSwitcher == null)
            return;

        var firstBombMaterial = (Material?) FirstBombMaterialField?.GetValue(conditionalMaterialSwitcher);
        var secondBombMaterial = (Material?) SecondBombMaterialField?.GetValue(conditionalMaterialSwitcher);
        
        FirstBombColor ??= firstBombMaterial?.GetColor(_SimpleColor);
        SecondBombColor ??= secondBombMaterial?.GetColor(_SimpleColor);

        var isDefaultColor = TweaksConfig.Instance.BombColor == DefaultColor;

        firstBombMaterial?.SetColor(_SimpleColor, isDefaultColor ? FirstBombColor ?? DefaultColor : TweaksConfig.Instance.BombColor);
        secondBombMaterial?.SetColor(_SimpleColor, isDefaultColor ? SecondBombColor ?? DefaultColor : TweaksConfig.Instance.BombColor);

    }

}