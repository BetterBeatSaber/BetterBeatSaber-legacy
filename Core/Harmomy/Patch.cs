using System.Reflection;

using HarmonyLib;

namespace BetterBeatSaber.Core.Harmomy; 

public static class Patch {

    private static readonly Harmony Harmony = new("xyz.betterbs");

    private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// Atm only for Prefix and Postfix methods
    /// </summary>
    public static void ApplyPatch(this object instance, Assembly assembly, string type, string method, BindingFlags? bindingFlags = null) {

        bindingFlags ??= DefaultBindingFlags;
        
        var targetMethod = GetTargetMethod(assembly, type, method, bindingFlags.Value);
        
        var prefix = instance.GetHarmonyMethod(HarmonyPatchType.Prefix);
        var postfix = instance.GetHarmonyMethod(HarmonyPatchType.Postfix);

        Harmony.Patch(targetMethod, prefix, postfix);

    }

    public static void RemovePatch(this object instance, Assembly assembly, string type, string method, BindingFlags? bindingFlags = null) {

        bindingFlags ??= DefaultBindingFlags;
        
        var targetMethod = GetTargetMethod(assembly, type, method, bindingFlags.Value);
        
        var prefix = instance.GetMethod(HarmonyPatchType.Prefix);
        var postfix = instance.GetMethod(HarmonyPatchType.Postfix);
        
        if (prefix != null)
            Harmony.Unpatch(targetMethod, prefix);
                    
        if (postfix != null)
            Harmony.Unpatch(targetMethod, postfix);
        
    }
    
    public static MethodInfo? GetMethod(this object instance, HarmonyPatchType patchType) {
        return instance.GetType().GetMethod(patchType.ToString(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
    }
    
    public static HarmonyMethod? GetHarmonyMethod(this object instance, HarmonyPatchType patchType) {
        var method = GetMethod(instance, patchType);
        return method != null ? new HarmonyMethod(method) : null;
    }

    private static MethodInfo? GetTargetMethod(Assembly assembly, string type, string method, BindingFlags bindingFlags) {
        return assembly.GetType(type).GetMethod(method, bindingFlags);
    }

}