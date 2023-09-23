using UnityEngine;

using Color = UnityEngine.Color;

namespace BetterBeatSaber.Core.Utilities; 

public sealed class RGB : MonoBehaviour {

    public static Color Color0 { get; private set; }
    public static Color Color1 { get; private set; }
    
    public static float Hue0 { get; private set; }

    public static void Instantiate() {
        var gameObject = new GameObject("RGB");
        gameObject.AddComponent<RGB>();
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        
        var t = Time.time / 5f;
        var hue = Mathf.Clamp(t % 2f >= 1f ? 1f - t % 1f : t % 1f, .05f, .9f);

        Hue0 = hue;

        Color0 = Color.HSVToRGB(Hue0, 1f, 1f);
        Color1 = Color.HSVToRGB(hue + .05f, 1f, 1f);
        
    }

}