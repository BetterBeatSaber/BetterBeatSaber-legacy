using HarmonyLib;

using UnityEngine;

namespace BetterBeatSaber.Core.Patches; 

/**
 * Cause Privacy is important!!!
 *
 * Also it maybe helps to prevent charges for Unity's new price model
 */

[HarmonyPatch(typeof(SystemInfo), nameof(SystemInfo.deviceName), MethodType.Getter)]
public static class DeviceNamePatch {

    [HarmonyPriority(int.MaxValue)]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once RedundantAssignment
    private static bool Prefix(ref string __result) {
        __result = "Ur-Moms-PC";
        return false;
    }
    
}

[HarmonyPatch(typeof(SystemInfo), nameof(SystemInfo.deviceModel), MethodType.Getter)]
public static class DeviceModelPatch {

    [HarmonyPriority(int.MaxValue)]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once RedundantAssignment
    private static bool Prefix(ref string __result) {
        __result = "Ur-Moms-PC";
        return false;
    }
    
}

[HarmonyPatch(typeof(SystemInfo), nameof(SystemInfo.deviceUniqueIdentifier), MethodType.Getter)]
public static class DeviceUniqueIdentifierPatch {

    [HarmonyPriority(int.MaxValue)]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once RedundantAssignment
    private static bool Prefix(ref string __result) {
        __result = "BC4B952B-F0C0-487B-9B22-313DC58821E5";
        return false;
    }
    
}

[HarmonyPatch(typeof(SystemInfo), nameof(SystemInfo.graphicsDeviceName), MethodType.Getter)]
public static class GraphicsDeviceNamePatch {

    [HarmonyPriority(int.MaxValue)]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once RedundantAssignment
    private static bool Prefix(ref string __result) {
        __result = "Ur-Moms-GraphicsCard";
        return false;
    }
    
}

// TODO: FIX!!!!

/*[HarmonyPatch(typeof(SystemInfo), nameof(SystemInfo.graphicsDeviceID), MethodType.Getter)]
public static class GraphicsDeviceIDPatch {

    [HarmonyPriority(int.MaxValue)]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once RedundantAssignment
    private static bool Prefix(ref string __result) {
        __result = "BC4B952B-F0C0-487B-9B22-313DC58821E5";
        return false;
    }
    
}*/