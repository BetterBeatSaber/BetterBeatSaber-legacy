using System;
using System.Collections.Generic;

using HarmonyLib;

using Zenject;

namespace BetterBeatSaber.Core.Patches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

[HarmonyPatch(typeof(Context))]
[HarmonyPatch("InstallInstallers")]
[HarmonyPatch(new[] { typeof(List<InstallerBase>), typeof(List<Type>), typeof(List<ScriptableObjectInstaller>), typeof(List<MonoInstaller>), typeof(List<MonoInstaller>) })]
internal static class ContextInstallInstallersPatch {

    internal static event Action<Context>? Install;

    private static readonly HashSet<Context> RecentlyInstalledDecorators = new();
    
    [HarmonyPrefix]
    [HarmonyPriority(int.MaxValue)]
    private static void Prefix(ref Context __instance) {
        
        if (RecentlyInstalledDecorators.Contains(__instance)) {
            RecentlyInstalledDecorators.Remove(__instance);
            return;
        }

        if (__instance is SceneDecoratorContext decorator)
            RecentlyInstalledDecorators.Add(decorator);

        Install?.Invoke(__instance);
        
    }
    
}