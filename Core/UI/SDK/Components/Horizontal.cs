using System.Xml;

using UnityEngine;
using UnityEngine.UI;

namespace BetterBeatSaber.Core.UI.SDK.Components; 

public class Horizontal : Panel {

    public override GameObject Create(Transform parent, XmlNode node) {
        var gameObject = base.Create(parent, node);
        gameObject.name = nameof(Horizontal);
        gameObject.AddComponent<HorizontalLayoutGroup>();
        return gameObject;
    }

}