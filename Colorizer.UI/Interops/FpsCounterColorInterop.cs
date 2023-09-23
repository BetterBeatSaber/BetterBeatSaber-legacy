using BetterBeatSaber.Core.Harmomy;
using BetterBeatSaber.Core.Harmomy.Dynamic;
using BetterBeatSaber.Core.Utilities;

using UnityEngine;

namespace BetterBeatSaber.Colorizer.UI.Interops; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

[DynamicPluginPatch("FPSCounter", "FPS_Counter.Converters.FpsTargetPercentageColorValueConverter", "Convert")]
public sealed class FpsCounterColorInterop : DynamicPatch {

    private const float RGBThreshold = .98f;
    
    internal static bool _rgb;
    
    public FpsCounterColorInterop(bool enabled) : base(enabled) { }
    
    private static readonly Color Orange = new(1, .64f, 0);
    
    // ReSharper disable once RedundantAssignment
    private static bool Prefix(float fpsTargetPercentage, ref Color __result) {

        _rgb = fpsTargetPercentage > RGBThreshold;
        
        __result = fpsTargetPercentage switch {
            > RGBThreshold => RGB.Color0,
            > .95f => Color.green,
            > .7f => Color.yellow,
            > .5f => Orange,
            _ => Color.red
        };

        return false;
        
    }
    
}