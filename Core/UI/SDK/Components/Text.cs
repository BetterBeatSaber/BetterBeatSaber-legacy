using System.Reflection;
using System.Xml;

using BetterBeatSaber.Core.UI.SDK.Attributes;

using JetBrains.Annotations;

using TMPro;

using UnityEngine;

namespace BetterBeatSaber.Core.UI.SDK.Components; 

public class Text : Component {

    private TextMeshProUGUI? _textMesh;

    private float _size = 4f;
    public float Size {
        // ReSharper disable once Unity.NoNullPropagation
        get => _textMesh?.fontSize ?? _size;
        set {
            _size = value;
            if (_textMesh != null)
                _textMesh.fontSize = value;
        }
    }

    private bool _rich;
    public bool Rich {
        // ReSharper disable once Unity.NoNullPropagation
        get => _textMesh?.richText ?? _rich;
        set {
            _rich = value;
            if (_textMesh != null)
                _textMesh.richText = value;
        }
    }
    
    public override GameObject Create(Transform parent, XmlNode node) {
        
        GameObject = new GameObject(nameof(Text)) {
            layer = BeatSaberUI.Layer
        };

        GameObject.transform.SetParent(parent, false);

        _textMesh = GameObject.AddComponent<TextMeshProUGUI>();
        
        _textMesh.font = BeatSaberMarkupLanguage.BeatSaberUI.MainTextFont;
        var mf = typeof(BeatSaberUI).GetProperty("MainUIFontMaterial", BindingFlags.Static | BindingFlags.NonPublic)?
            .GetValue(null);
        if(mf != null)
            _textMesh.fontSharedMaterial = (Material) mf;
        
        _textMesh.fontSize = _size;
        _textMesh.richText = _rich;
        _textMesh.color = Color.white;
        _textMesh.text = node.InnerText;

        _textMesh.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        _textMesh.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

        return GameObject;
        
    }

}