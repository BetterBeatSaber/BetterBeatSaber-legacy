using System;

namespace BetterBeatSaber.Core.UI.SDK.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class ComponentAttribute : Attribute {

    public string Id { get; }

    public ComponentAttribute(string id) {
        Id = id;
    }

}