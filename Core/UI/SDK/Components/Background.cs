using System.Xml;

using BeatSaberMarkupLanguage.Components;

using UnityEngine;
using UnityEngine.UI;

namespace BetterBeatSaber.Core.UI.SDK.Components; 

public class Background : Component {

    public override GameObject Create(Transform parent, XmlNode node) {
        
        GameObject = new GameObject(nameof(Background)) {
            layer = BeatSaberUI.Layer
        };

        GameObject.transform.SetParent(parent, false);
        
        GameObject.AddComponent<ContentSizeFitter>();
        GameObject.AddComponent<Backgroundable>();

        var rectTransform = GameObject.transform as RectTransform;
        if (rectTransform == null)
            return GameObject;
        
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.sizeDelta = new Vector2(0, 0);

        return GameObject;
        
    }

}