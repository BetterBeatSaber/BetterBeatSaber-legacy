using BetterBeatSaber.Core.Harmomy;
using BetterBeatSaber.Core.Harmomy.Dynamic;
using BetterBeatSaber.Core.Provider;

using HMUI;

using TMPro;

namespace BetterBeatSaber.Colorizer.UI.Interops; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

[DynamicPluginPatch("FPSCounter", "FPS_Counter.Counters.FpsCounterCountersPlus", "CounterInit")]
public sealed class FpsCounterInterop : DynamicPatch {

    public FpsCounterInterop(bool enabled) : base(enabled) { }
    
    private static void Postfix(ref TMP_Text ____counterText, ref ImageView ____ringImage) {
        ____counterText.font = AssetProvider.Instance?.DefaultFontBloom;
        ____ringImage.material = AssetProvider.Instance?.DefaultUIMaterial;
    }
    
}