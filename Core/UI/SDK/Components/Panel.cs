using System.Xml;

using BeatSaberMarkupLanguage.Components;

using UnityEngine;
using UnityEngine.UI;

namespace BetterBeatSaber.Core.UI.SDK.Components; 

public class Panel : Component, Component.IParent {

    public float? PrefWidth { get; set; }
    public float? PrefHeight { get; set; }

    public override GameObject Create(Transform parent, XmlNode node) {
        
        GameObject = new GameObject(nameof(Panel)) {
            layer = BeatSaberUI.Layer
        };

        GameObject.transform.SetParent(parent, false);
        
        var contentSizeFitter = GameObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        GameObject.AddComponent<Backgroundable>();

        var rectTransform = GameObject.transform as RectTransform;
        if (rectTransform != null) {
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.sizeDelta = new Vector2(0, 0);
        }
        
        var layoutElement = GameObject.AddComponent<LayoutElement>();
        
        if (PrefWidth != null)
            layoutElement.preferredWidth = PrefWidth.Value;

        if (PrefHeight != null)
            layoutElement.preferredHeight = PrefHeight.Value;
        
        return GameObject;
        
    }

}