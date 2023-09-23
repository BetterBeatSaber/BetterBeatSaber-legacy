using System;
using System.Linq;

using TMPro;

using UnityEngine;

namespace BetterBeatSaber.Core.Provider;

public sealed class AssetProvider : IDisposable {

    public static AssetProvider? Instance;
    
    public readonly TMP_FontAsset DefaultFont;
    public readonly TMP_FontAsset DefaultFontBloom;

    public readonly Material DefaultUIMaterial;
    public readonly Material DistanceFieldMaterial;

    public AssetProvider() {

        Instance = this;

        // ReSharper disable once StringLiteralTypo
        var defaultFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault(x => x.name == "Teko-Medium SDF");
        if (defaultFont != null) {

            DefaultFont = defaultFont;

            // find a way for 1.29+ cause of new unity version how to clone the "DefaultFont"
            var bloomFont = TMP_FontAsset.CreateFontAsset(DefaultFont.sourceFontFile);

            bloomFont.name = "Default Font (Bloom)";
            bloomFont.material.shader = Shader.Find("TextMeshPro/Distance Field");

            DefaultFontBloom = bloomFont;

        } else {
            throw new Exception("Failed to find default font!!!");
        }

        DefaultUIMaterial = new Material(Shader.Find("UI/Default"));
        DistanceFieldMaterial = new Material(Shader.Find("TextMeshPro/Distance Field"));
        
    }

    public void Dispose() {
        Instance = null;
        #if PRE_129
        GameObject.Destroy(DefaultFontBloom);
        #endif
    }

}