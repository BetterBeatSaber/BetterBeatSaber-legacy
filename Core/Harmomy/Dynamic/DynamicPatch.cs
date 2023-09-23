using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using HarmonyLib;

using IPA.Loader;

namespace BetterBeatSaber.Core.Harmomy.Dynamic; 

public abstract class DynamicPatch {
    
    // ReSharper disable once ConvertToConstant.Global
    public static readonly BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
    
    public static readonly Harmony DynamicHarmony = new("xyz.betterbs.dynamic");

    private bool _patched;
    
    private bool _enabled;
    public bool Enabled {
        get => _enabled;
        set {
            if (value != _enabled) {
                if (value) {
                    Patch();
                } else if(_patched) {
                    Unpatch();
                }
            }
            _enabled = value;
        }
    }

    private void Patch() {
        
        var prefix = this.GetHarmonyMethod(HarmonyPatchType.Prefix);
        var postfix = this.GetHarmonyMethod(HarmonyPatchType.Postfix);
        var transpiler = this.GetHarmonyMethod(HarmonyPatchType.Transpiler);

        foreach (var targetMethod in GetTargetMethods())
            DynamicHarmony.Patch(targetMethod, prefix, postfix, transpiler: transpiler);
        
        _patched = true;
        
    }

    private void Unpatch() {
        
        var prefix = this.GetMethod(HarmonyPatchType.Prefix);
        var postfix = this.GetMethod(HarmonyPatchType.Postfix);
        var transpiler = this.GetMethod(HarmonyPatchType.Transpiler);

        foreach (var targetMethod in GetTargetMethods()) {
            
            if (prefix != null)
                DynamicHarmony.Unpatch(targetMethod, prefix);
                    
            if (postfix != null)
                DynamicHarmony.Unpatch(targetMethod, postfix);
            
            if (transpiler != null)
                DynamicHarmony.Unpatch(targetMethod, transpiler);
            
        }

        _patched = false;

    }

    private IEnumerable<MethodBase> GetTargetMethods() {
        
        var targetMethods = new List<MethodBase>();
        if (TargetMethod != null)
            targetMethods.Add(TargetMethod);
        
        if(TargetMethods != null)
            targetMethods.AddRange(TargetMethods);
        
        targetMethods.AddRange(GetType().GetCustomAttributes().Where(attribute => attribute is DynamicPatchAttribute).Cast<DynamicPatchAttribute>().Select(dynamicPatch => dynamicPatch.Type.GetMethod(dynamicPatch.MethodName, dynamicPatch.BindingFlags)).Cast<MethodBase>().ToArray());

        var pluginPatchAttribute = GetType().GetCustomAttribute<DynamicPluginPatchAttribute>();
        if (pluginPatchAttribute != null && PluginManager.GetPluginFromId(pluginPatchAttribute.Plugin) != null) {
            targetMethods.Add(PluginManager
                              .GetPluginFromId(pluginPatchAttribute.Plugin)
                              .Assembly
                              .GetType(pluginPatchAttribute.Type)
                              .GetMethod(pluginPatchAttribute.Method, DefaultBindingFlags));
        }
        
        return targetMethods;
    }

    protected DynamicPatch(bool enabled) {
        Enabled = enabled;
    }

    protected virtual MethodBase? TargetMethod { get; } = null;
    protected virtual MethodBase[]? TargetMethods { get; } = null;

    /*public static void Apply<T>() {
        
    }

    public static void Unapply<T>() {
        
    }*/
    
}