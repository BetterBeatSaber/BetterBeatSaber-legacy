using BetterBeatSaber.Core.Utilities;

using UnityEngine;

namespace BetterBeatSaber.Colorizer.Game.Colorizer; 

public sealed class BombColorizer : MonoBehaviour {

    private static readonly int SimpleColor = Shader.PropertyToID("_SimpleColor");
    
    private Material? _sharedMaterial;

    private void Start() {
        _sharedMaterial = gameObject.GetComponentInChildren<Renderer>().sharedMaterial;
    }

    private void Update() {
        if(_sharedMaterial != null)
            _sharedMaterial.SetColor(SimpleColor, RGB.Color0);
    }

}