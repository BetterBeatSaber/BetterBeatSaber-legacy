using BetterBeatSaber.Core.Harmomy;
using BetterBeatSaber.Core.Harmomy.Dynamic;
using BetterBeatSaber.Core.Provider;

using TMPro;

namespace BetterBeatSaber.Colorizer.UI.Interops; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

[DynamicPluginPatch("PBOT", "PBOT.Managers.DeltaRankCounterVisualManager", "CounterInit")]
public sealed class PbotInterop : DynamicPatch {

    public PbotInterop(bool enabled) : base(enabled) { }
    
    private static void Postfix(ref TMP_Text ____text) {
        ____text.font = AssetProvider.Instance?.DefaultFontBloom;
    }

}