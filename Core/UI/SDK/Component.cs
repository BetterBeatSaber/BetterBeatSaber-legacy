using System.Xml;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.UI.SDK.Attributes;

using JetBrains.Annotations;

using UnityEngine;

namespace BetterBeatSaber.Core.UI.SDK; 

public abstract class Component {

    public string? Id { get; set; }
    
    [UsedImplicitly]
    [IgnoreProperty]
    public GameObject? GameObject { get; set; }
    
    public abstract GameObject Create(Transform parent, XmlNode node);

    public virtual void PostCreation(ParseContext context) { }
    
    protected void SetActiveIfNot() => GameObject.SetActiveIfNot();
    protected void SetInactiveIfNot() => GameObject.SetInactiveIfNot();
    protected void SetActiveIf(bool when, bool setInactiveIfNot = true) => GameObject.SetActiveIf(when, setInactiveIfNot);
    
    public interface IParent { }
    
}