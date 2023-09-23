using System;

using BetterBeatSaber.Core.Utilities;

using IPA.Utilities;

using UnityEngine;

namespace BetterBeatSaber.Colorizer.Game.Colorizer; 

public sealed class BurnMarksColorizer : MonoBehaviour {

    public SaberBurnMarkArea? saberBurnMarkArea;
    
    private LineRenderer[]? _lineRenderers;
    
    private void Start() {
        if (saberBurnMarkArea != null)
            _lineRenderers = saberBurnMarkArea.GetField<LineRenderer[], SaberBurnMarkArea>("_lineRenderers");
    }

    private void Update() {
        
        if (_lineRenderers == null)
            return;
        
        foreach (var renderer in _lineRenderers) {
            renderer.startColor = RGB.Color0;
            renderer.endColor = RGB.Color1;
        }
        
    }

}