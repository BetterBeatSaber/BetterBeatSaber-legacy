using BetterBeatSaber.Core.Extensions;

using HarmonyLib;

namespace BetterBeatSaber.Core.UI.SDK.Patches; 

[HarmonyPatch(typeof(MainSettingsMenuViewControllersInstaller), nameof(MainSettingsMenuViewControllersInstaller.InstallBindings))]
public static class MainSettingsMenuViewControllersInstallerPatch {

    // ReSharper disable once SuggestBaseTypeForParameter
    // ReSharper disable once InconsistentNaming
    [HarmonyPriority(int.MaxValue)]
    [HarmonyPrefix]
    private static void Prefix(MainSettingsMenuViewControllersInstaller __instance) =>
        BeatSaberUI.Instance.DiContainer = __instance.GetContainer();

}