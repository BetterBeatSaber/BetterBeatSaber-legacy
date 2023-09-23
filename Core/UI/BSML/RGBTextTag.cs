using BeatSaberMarkupLanguage.Tags;

using BetterBeatSaber.Core.Utilities;

using HMUI;

using UnityEngine;

namespace BetterBeatSaber.Core.UI.BSML; 

public sealed class RGBTextTag : TextTag {

    public override string[] Aliases { get; } = { "rgb-text", "text-rbg", "rgb" };

    public override GameObject? CreateObject(Transform parent) {
        
        var text = base.CreateObject(parent);
        if (text == null)
            return null;
        
        text.AddComponent<RGBTextTagColorizer>();
        
        return text;
        
    }

    public sealed class RGBTextTagColorizer : MonoBehaviour {

        private CurvedTextMeshPro? _text;
        
        private void Start() {
            _text = gameObject.GetComponentInChildren<CurvedTextMeshPro>();
        }

        private void Update() {
            if (_text != null)
                _text.color = RGB.Color0;
        }

    }

}