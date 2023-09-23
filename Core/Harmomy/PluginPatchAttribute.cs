using System;

namespace BetterBeatSaber.Core.Harmomy; 

[AttributeUsage(AttributeTargets.Class)]
public class PluginPatchAttribute : Attribute {

    public string Plugin { get; }
    public string Type { get; }
    public string Method { get; }

    public PluginPatchAttribute(string plugin, string type, string method) {
        Plugin = plugin;
        Type = type;
        Method = method;
    }

}