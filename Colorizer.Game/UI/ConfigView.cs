using System.Collections.Generic;
using System.Linq;

using BeatSaberMarkupLanguage.Attributes;

using BetterBeatSaber.Colorizer.Game.Config;
using BetterBeatSaber.Core.Enums;
using BetterBeatSaber.Core.UI;
using BetterBeatSaber.Core.UI.Main;

using IPA.Loader;

using UnityEngine;

namespace BetterBeatSaber.Colorizer.Game.UI;

#if DEBUG
[HotReload(RelativePathToLayout = "./ConfigView.bsml")]
#endif
public sealed class ConfigView : View<ConfigView> {

    private static readonly Dictionary<Visibility, string> VisibilitiesToNames = new() {
        { Visibility.All, "VR and Desktop" },
        { Visibility.VROnly, "VR Only" },
        { Visibility.DesktopOnly, "Desktop Only" }
    };

    [UIValue(nameof(visibilities))]
    public List<object> visibilities = VisibilitiesToNames.Keys.Cast<object>().ToList();

    #region UI Elements

    [UIObject("custom-notes")]
    private readonly GameObject _customNotes = null!;

    #endregion
    
    #region UI Actions

    [UIAction("#post-parse")]
    public void PostParse() {
        _customNotes.SetActive(PluginManager.GetPluginFromId("Custom Notes") != null);
    }

    #endregion
    
    #region Notes

    public bool NotesColorize {
        get => GameColorizerConfig.Instance.Notes.Colorize.Enabled;
        set {
            GameColorizerConfig.Instance.Notes.Colorize.Enabled = value;
            GameColorizerConfig.Instance.Updated = true;
        }
    }
    
    public float NotesOutlinesWidth {
        get => GameColorizerConfig.Instance.Notes.OutlinesWidth;
        set {
            GameColorizerConfig.Instance.Notes.OutlinesWidth = value;
            GameColorizerConfig.Instance.Updated = true;
        }
    }
    
    public Visibility NotesVisibility {
        get => GameColorizerConfig.Instance.Notes.Visibility;
        set {
            GameColorizerConfig.Instance.Notes.Visibility = value;
            GameColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool NotesGlow {
        get => GameColorizerConfig.Instance.Notes.Glow;
        set {
            GameColorizerConfig.Instance.Notes.Glow = value;
            GameColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool ColorizeDebris {
        get => GameColorizerConfig.Instance.Notes.ColorizeDebris;
        set {
            GameColorizerConfig.Instance.Notes.ColorizeDebris = value;
            GameColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool ColorizeCustomNotes {
        get => GameColorizerConfig.Instance.Notes.ColorizeCustomNotes.Enabled;
        set {
            GameColorizerConfig.Instance.Notes.ColorizeCustomNotes.Enabled = value;
            GameColorizerConfig.Instance.Updated = true;
        }
    }
    
    #endregion

    #region Bombs

    public bool BombsColorize {
        get => GameColorizerConfig.Instance.Bombs.Colorize.Enabled;
        set {
            GameColorizerConfig.Instance.Bombs.Colorize.Enabled = value;
            GameColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool BombsColorizeOutlines {
        get => GameColorizerConfig.Instance.Bombs.ColorizeOutlines.Enabled;
        set {
            GameColorizerConfig.Instance.Bombs.ColorizeOutlines.Enabled = value;
            GameColorizerConfig.Instance.Updated = true;
        }
    }
    
    public float BombsOutlinesWidth {
        get => GameColorizerConfig.Instance.Bombs.OutlinesWidth;
        set {
            GameColorizerConfig.Instance.Bombs.OutlinesWidth = value;
            GameColorizerConfig.Instance.Updated = true;
        }
    }
    
    public Visibility BombsVisibility {
        get => GameColorizerConfig.Instance.Bombs.Visibility;
        set {
            GameColorizerConfig.Instance.Bombs.Visibility = value;
            GameColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool BombsGlow {
        get => GameColorizerConfig.Instance.Bombs.Glow;
        set {
            GameColorizerConfig.Instance.Bombs.Glow = value;
            GameColorizerConfig.Instance.Updated = true;
        }
    }
    
    #endregion

    #region Obstacles

    public bool ObstaclesColorize {
        get => GameColorizerConfig.Instance.Obstacles.Colorize.Enabled;
        set {
            GameColorizerConfig.Instance.Obstacles.Colorize.Enabled = value;
            GameColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool ObstaclesTransparent {
        get => GameColorizerConfig.Instance.Obstacles.Transparent;
        set {
            GameColorizerConfig.Instance.Obstacles.Transparent = value;
            GameColorizerConfig.Instance.Updated = true;
        }
    }

    #endregion

    #region Extras

    /*public bool ColorizeArcs {
        get => TweaksConfig.Instance.ColorizeArcs;
        set {
            TweaksConfig.Instance.ColorizeArcs = value;
            TweaksConfig.Instance.Updated = true;
        }
    }*/

    public bool ColorizePlayersPlaceBorder {
        get => GameColorizerConfig.Instance.ColorizePlayersPlace;
        set {
            GameColorizerConfig.Instance.ColorizePlayersPlace = value;
            GameColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool ColorizeFeet {
        get => GameColorizerConfig.Instance.ColorizeFeet;
        set {
            if(value != GameColorizerConfig.Instance.ColorizeFeet)
                ModuleFlowController.RequiresSoftRestart = true;
            GameColorizerConfig.Instance.ColorizeFeet = value;
            GameColorizerConfig.Instance.Updated = true;
        }
    }
    
    public bool ColorizeBurnMarks {
        get => GameColorizerConfig.Instance.ColorizeBurnMarks.Enabled;
        set {
            GameColorizerConfig.Instance.ColorizeBurnMarks.Enabled = value;
            GameColorizerConfig.Instance.Updated = true;
        }
    }

    #endregion
    
}