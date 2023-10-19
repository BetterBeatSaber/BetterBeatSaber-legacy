using System.Xml;

using BetterBeatSaber.Core.UI.SDK.Attributes;

using HMUI;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

namespace BetterBeatSaber.Core.UI.SDK.Components; 

public class Image : Component {

    [UsedImplicitly]
    [IgnoreProperty]
    protected ImageView? ImageView { get; private set; }
    
    public override GameObject Create(Transform parent, XmlNode node) {
        
        GameObject = new GameObject(nameof(Image)) {
            layer = BeatSaberUI.Layer
        };

        ImageView = GameObject.AddComponent<ImageView>();
        
        ImageView.rectTransform.SetParent(parent, false);
        ImageView.rectTransform.sizeDelta = new Vector2(20f, 20f);
        
        ImageView.preserveAspect = true;
        
        //ImageView.material = Utilities.ImageResources.NoGlowMat;
        //ImageView.sprite = Utilities.ImageResources.BlankSprite;

        GameObject.AddComponent<LayoutElement>();

        return GameObject;

    }

}