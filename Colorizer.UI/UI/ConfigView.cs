using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;

using BetterBeatSaber.Colorizer.UI.Config;
using BetterBeatSaber.Core.UI;
using BetterBeatSaber.Core.UI.Main;

using UnityEngine;

namespace BetterBeatSaber.Colorizer.UI.UI;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

#if DEBUG
[HotReload(RelativePathToLayout = "./ConfigView.bsml")]
#endif
public sealed class ConfigView : View<ConfigView> {

    #region UI Objects

    [UIComponent("rank-ss-color")]
    private readonly GenericInteractableSetting _rankSSColorSetting = null!;

    [UIComponent("rank-s-color")]
    private readonly GenericInteractableSetting _rankSColorSetting = null!;
    
    [UIComponent("rank-a-color")]
    private readonly GenericInteractableSetting _rankAColorSetting = null!;
    
    [UIComponent("rank-b-color")]
    private readonly GenericInteractableSetting _rankBColorSetting = null!;
    
    [UIComponent("rank-c-color")]
    private readonly GenericInteractableSetting _rankCColorSetting = null!;
    
    [UIComponent("rank-d-color")]
    private readonly GenericInteractableSetting _rankDColorSetting = null!;
    
    [UIComponent("rank-e-color")]
    private readonly GenericInteractableSetting _rankEColorSetting = null!;
    
    #endregion

    #region Colorizers

    public bool ColorizeButtons {
        get => UIColorizerConfig.Instance.ColorizeButtons;
        set {
            if(value != UIColorizerConfig.Instance.ColorizeButtons)
                ModuleFlowController.RequiresSoftRestart = true;
            UIColorizerConfig.Instance.ColorizeButtons = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool ColorizeMenuButtons {
        get => UIColorizerConfig.Instance.ColorizeMenuButtons;
        set {
            if(value != UIColorizerConfig.Instance.ColorizeMenuButtons)
                ModuleFlowController.RequiresSoftRestart = true;
            UIColorizerConfig.Instance.ColorizeMenuButtons = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }

    #endregion
    
    #region Hud Modifier

    public bool ComboHudModifierGlow {
        get => UIColorizerConfig.Instance.ComboHudModifier.Glow;
        set {
            UIColorizerConfig.Instance.ComboHudModifier.Glow = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool EnergyHudModifierGlow {
        get => UIColorizerConfig.Instance.EnergyHudModifier.Glow;
        set {
            UIColorizerConfig.Instance.EnergyHudModifier.Glow = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool EnergyHudModifierShakeOnComboBreak {
        get => UIColorizerConfig.Instance.EnergyHudModifier.ShakeOnComboBreak;
        set {
            UIColorizerConfig.Instance.EnergyHudModifier.ShakeOnComboBreak = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool MultiplierHudModifierGlow {
        get => UIColorizerConfig.Instance.MultiplierHudModifier.Glow;
        set {
            UIColorizerConfig.Instance.MultiplierHudModifier.Glow = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool ProgressHudModifierGlow {
        get => UIColorizerConfig.Instance.ProgressHudModifier.Glow;
        set {
            UIColorizerConfig.Instance.ProgressHudModifier.Glow = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool ScoreHudModifierGlow {
        get => UIColorizerConfig.Instance.ScoreHudModifier.Glow;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.Glow = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }

    #endregion

    #region Custom Ranks

    #region SS

    public string RankSSName {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.SS].Name;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.SS].Name = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public Color RankSSColor {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.SS].Color;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.SS].Color = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }

    public bool RankSSRGB {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.SS].RGB;
        set {
            _rankSSColorSetting.interactable = !value;
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.SS].RGB = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool RankSSGlow {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.SS].Glow;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.SS].Glow = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    #endregion

    #region S

    public string RankSName {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.S].Name;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.S].Name = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public Color RankSColor {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.S].Color;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.S].Color = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool RankSRGB {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.S].RGB;
        set {
            _rankSColorSetting.interactable = !value;
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.S].RGB = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool RankSGlow {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.S].Glow;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.S].Glow = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }

    #endregion

    #region A

    public string RankAName {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.A].Name;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.A].Name = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public Color RankAColor {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.A].Color;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.A].Color = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool RankARGB {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.A].RGB;
        set {
            _rankAColorSetting.interactable = !value;
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.A].RGB = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool RankAGlow {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.A].Glow;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.A].Glow = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }

    #endregion
    
    #region B

    public string RankBName {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.B].Name;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.B].Name = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public Color RankBColor {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.B].Color;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.B].Color = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool RankBRGB {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.B].RGB;
        set {
            _rankBColorSetting.interactable = !value;
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.B].RGB = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool RankBGlow {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.B].Glow;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.B].Glow = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }

    #endregion
    
    #region C

    public string RankCName {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.C].Name;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.C].Name = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public Color RankCColor {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.C].Color;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.C].Color = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool RankCRGB {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.C].RGB;
        set {
            _rankCColorSetting.interactable = !value;
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.C].RGB = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool RankCGlow {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.C].Glow;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.C].Glow = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }

    #endregion
    
    #region D

    public string RankDName {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.D].Name;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.D].Name = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public Color RankDColor {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.D].Color;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.D].Color = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool RankDRGB {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.D].RGB;
        set {
            _rankDColorSetting.interactable = !value;
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.D].RGB = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }

    public bool RankDGlow {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.D].Glow;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.D].Glow = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    #endregion
    
    #region E

    public string RankEName {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.E].Name;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.E].Name = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public Color RankEColor {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.E].Color;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.E].Color = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool RankERGB {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.E].RGB;
        set {
            _rankEColorSetting.interactable = !value;
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.E].RGB = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool RankEGlow {
        get => UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.E].Glow;
        set {
            UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks[RankModel.Rank.E].Glow = value;
            UIColorizerConfig.Instance.Updated = true;
        }
    }

    #endregion
    
    #endregion
    
}