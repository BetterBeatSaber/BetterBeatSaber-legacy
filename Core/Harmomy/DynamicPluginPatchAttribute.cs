using System;

namespace BetterBeatSaber.Core.Harmomy; 

[AttributeUsage(AttributeTargets.Class)]
public sealed class DynamicPluginPatchAttribute : PluginPatchAttribute {

    public DynamicPluginPatchAttribute(string plugin, string type, string method) : base(plugin, type, method) { }

}