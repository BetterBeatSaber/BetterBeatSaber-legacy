using System.Text.RegularExpressions;

using TMPro;

using UnityEngine;

namespace BetterBeatSaber.Core.TextMeshPro; 

public static class TextMeshProAddon {

    public static void Apply(TMP_Text text) {
        
        text.text = Regex.Replace(text.text, @"(<rgb>(.*)<\/rgb>)", "<link=\"rgb\">$2</link>");
        if (text.text.Contains("<link=\"rgb\">")) {
            text.gameObject.AddComponent<RGBRichTag>();
        }
        
        text.text = Regex.Replace(text.text, @"(<rgb-gradient>(.*)<\/rgb-gradient>)", "<link=\"rgb-gradient\">$2</link>");
        if (text.text.Contains("<link=\"rgb-gradient\">")) {
            text.gameObject.AddComponent<RGBGradientRichTag>();
        }
        
    }

    public static void RemoveAny(TMP_Text text) {
        RemoveAny(text.gameObject);
    }

    public static void RemoveAny(GameObject gameObject) {
        MonoBehaviour? component;
        if ((component = gameObject.GetComponentInChildren<RGBRichTag>()) != null)
            Object.Destroy(component);
        if ((component = gameObject.GetComponentInChildren<RGBGradientRichTag>()) != null)
            Object.Destroy(component);
    }

}