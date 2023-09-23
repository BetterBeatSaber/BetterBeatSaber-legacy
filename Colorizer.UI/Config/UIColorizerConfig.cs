using BetterBeatSaber.Colorizer.UI.HudModifier;
using BetterBeatSaber.Colorizer.UI.Interops;
using BetterBeatSaber.Core.Config;

namespace BetterBeatSaber.Colorizer.UI.Config;

public sealed class UIColorizerConfig : Config<UIColorizerConfig> {

    public bool HideEditorButton { get; set; } = true;
    public bool HidePromotionButton { get; set; } = true;

    public bool ColorizeButtons { get; set; } = true;
    public bool ColorizeMenuButtons { get; set; } = false;

    public PbotInterop ColorizePbotCounter { get; set; } = new(true);
    
    public FpsCounterInterop ColorizeFpsCounter { get; set; } = new(true);
    public FpsCounterColorInterop ColorizeFpsCounter2 { get; set; } = new(true);
    public FpsCounterTickInterop ColorizeFpsCounter3 { get; set; } = new(true);
    
    #region Hud Modifiers

    public HudModifier.HudModifier.BaseOptions ComboHudModifier { get; set; } = new();

    public EnergyHudModifier.Options EnergyHudModifier { get; set; } = new();

    public HudModifier.HudModifier.BaseOptions MultiplierHudModifier { get; set; } = new();

    public HudModifier.HudModifier.BaseOptions ProgressHudModifier { get; set; } = new();

    public ScoreHudModifier.Options ScoreHudModifier { get; set; } = new();

    #endregion

}