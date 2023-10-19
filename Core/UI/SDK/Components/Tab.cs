using System.Xml;

using BetterBeatSaber.Core.UI.SDK.Attributes;

using JetBrains.Annotations;

using UnityEngine;

namespace BetterBeatSaber.Core.UI.SDK.Components; 

public sealed class Tab : Background {

    public string? Name { get; set; } = nameof(Tab);
    
    public bool Visible { get; set; } = true;

    [UsedImplicitly]
    [IgnoreProperty]
    public TabSelector? Selector { get; internal set; }
    
    public override GameObject Create(Transform parent, XmlNode node) {
        GameObject = base.Create(parent, node);
        GameObject.name = Name + nameof(Tab);
        return GameObject;
    }

}