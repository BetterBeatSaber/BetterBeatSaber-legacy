using System.Xml;

using UnityEngine;
using UnityEngine.UI;

namespace BetterBeatSaber.Core.UI.SDK.Components; 

public class Vertical : Panel {

    public override GameObject Create(Transform parent, XmlNode node) {
        var gameObject = base.Create(parent, node);
        gameObject.AddComponent<VerticalLayoutGroup>();
        return gameObject;
    }

}