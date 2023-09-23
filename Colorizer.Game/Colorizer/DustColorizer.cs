using System;

using BetterBeatSaber.Core.Utilities;

using UnityEngine;

namespace BetterBeatSaber.Colorizer.Game.Colorizer; 

public sealed class DustColorizer : MonoBehaviour {

    private ParticleSystem? _particleSystem;

    private void Start() {
        _particleSystem = GameObject.Find("DustPS").GetComponent<ParticleSystem>();
    }

    [Obsolete("Obsolete")]
    private void Update() {
        if (_particleSystem != null)
            _particleSystem.startColor = RGB.Color0;
    }

}