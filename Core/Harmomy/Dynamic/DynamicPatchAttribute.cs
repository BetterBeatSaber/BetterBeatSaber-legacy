using System;
using System.Reflection;

namespace BetterBeatSaber.Core.Harmomy.Dynamic; 

[AttributeUsage(AttributeTargets.Class)]
public sealed class DynamicPatchAttribute : Attribute {

    public readonly Type Type;
    public readonly string MethodName;
    public readonly BindingFlags BindingFlags;
    
    public DynamicPatchAttribute(Type type, string methodName) {
        Type = type;
        MethodName = methodName;
        BindingFlags = DynamicPatch.DefaultBindingFlags;
    }
    
    public DynamicPatchAttribute(Type type, string methodName, BindingFlags bindingFlags) {
        Type = type;
        MethodName = methodName;
        BindingFlags = bindingFlags;
    }

}