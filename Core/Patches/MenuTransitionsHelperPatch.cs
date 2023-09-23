using BetterBeatSaber.Core.Game;

using HarmonyLib;

namespace BetterBeatSaber.Core.Patches; 

[HarmonyPatch(typeof(MenuTransitionsHelper), nameof(MenuTransitionsHelper.RestartGame), MethodType.Normal)]
internal static class MenuTransitionsHelperPatch {

    [HarmonyPriority(int.MaxValue)]
    [HarmonyPrefix]
    private static void Prefix() => BeatSaber.TriggerSoftRestartEvent();

}