using BetterBeatSaber.Core.Utilities;

using UnityEngine;

namespace BetterBeatSaber.Colorizer.Game.Colorizer; 

public sealed class FeetColorizer : MonoBehaviour {

    private SpriteRenderer? _renderer;

    private void Start() {
        _renderer = GameObject.Find("PlayersPlace/Feet").GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if (_renderer != null)
            _renderer.color = RGB.Color0;
    }

}