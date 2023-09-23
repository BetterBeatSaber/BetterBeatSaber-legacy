using System;

using BetterBeatSaber.Colorizer.UI.Config;

using HarmonyLib;

namespace BetterBeatSaber.Colorizer.UI.Patches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

[HarmonyPatch(typeof(RankModel), nameof(RankModel.GetRankName))]
public static class RankModelNamePatch {

    [HarmonyPostfix]
    private static void Postfix(ref string __result) {
        Enum.TryParse<RankModel.Rank>(__result, out var rank);
        if (UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks.ContainsKey(rank) && UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks.TryGetValue(rank, out var config) && config.Enabled) {
            __result = config.Name;
        }
    }

}