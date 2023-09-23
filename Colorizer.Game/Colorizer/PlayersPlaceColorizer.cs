using BetterBeatSaber.Core.Utilities;

using UnityEngine;

namespace BetterBeatSaber.Colorizer.Game.Colorizer; 

public sealed class PlayersPlaceColorizer : MonoBehaviour {

    private RectangleFakeGlow? _rectangleFakeGlow;
    
    private void Start() {
        _rectangleFakeGlow = GameObject.Find("PlayersPlace/RectangleFakeGlow").GetComponent<RectangleFakeGlow>();
    }

    private void Update() {
        if (_rectangleFakeGlow != null)
            _rectangleFakeGlow.color = RGB.Color0;
    }

}