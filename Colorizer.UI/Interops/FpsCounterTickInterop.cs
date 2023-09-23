using BetterBeatSaber.Core.Harmomy;
using BetterBeatSaber.Core.Harmomy.Dynamic;
using BetterBeatSaber.Core.Utilities;

using HMUI;

using TMPro;

namespace BetterBeatSaber.Colorizer.UI.Interops; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

[DynamicPluginPatch("FPSCounter", "FPS_Counter.Counters.FpsCounterCountersPlus", "Tick")]
public sealed class FpsCounterTickInterop : DynamicPatch {

    public FpsCounterTickInterop(bool enabled) : base(enabled) { }

    private static void Postfix(ref TMP_Text ____counterText, ref ImageView ____ringImage) {
        
        if (!FpsCounterColorInterop._rgb)
            return;
        
        ____counterText.color = RGB.Color0;
        ____ringImage.color = RGB.Color0;
        
    }

}