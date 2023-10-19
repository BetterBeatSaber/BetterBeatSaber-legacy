using System.Xml;

using BetterBeatSaber.Core.UI.SDK.Attributes;

using JetBrains.Annotations;

using UnityEngine;

namespace BetterBeatSaber.Core.UI.SDK.Components; 

public sealed class Tab : Background, Component.IParent {

    private string _name = nameof(Tab);

    public string Name {
        get => _name;
        set {
            var refresh = value != _name;
            _name = value;
            if (refresh)
                Selector?.Refresh();
        }
    }

    private bool _visible = true;

    public bool Visible {
        get => _visible;
        set {
            var refresh = value != _visible;
            _visible = value;
            if (refresh)
                Selector?.Refresh();
        }
    }
    
    [UsedImplicitly]
    public string For { get; set; } = null!;

    [UsedImplicitly]
    [IgnoreProperty]
    public TabSelector? Selector { get; internal set; }
    
    public override GameObject Create(Transform parent, XmlNode node) {
        GameObject = base.Create(parent, node);
        GameObject.name = Name + nameof(Tab);
        return GameObject;
    }

}