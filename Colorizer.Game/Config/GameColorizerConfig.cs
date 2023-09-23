using BetterBeatSaber.Colorizer.Game.DynamicPatches;
using BetterBeatSaber.Core.Config;
using BetterBeatSaber.Core.Enums;
using BetterBeatSaber.Core.Harmomy.Dynamic;

namespace BetterBeatSaber.Colorizer.Game.Config; 

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable UnusedMember.Global

public sealed class GameColorizerConfig : Config<GameColorizerConfig> {

    public bool ColorizeFeet { get; set; } = true;
    public bool ColorizePlayersPlace { get; set; } = true;
    
    public BurnMarkPatch ColorizeBurnMarks { get; set; } = new(true);
    
    public bool ColorizeDust { get; set; } = true;
    
    public NotesConfig Notes { get; set; } = new();
    public BombsConfig Bombs { get; set; } = new();
    public ObstaclesConfig Obstacles { get; set; } = new();
    
    public sealed partial class NotesConfig : ColorizeConfig {

        public override DynamicPatch Colorize { get; set; } = new BaseNoteVisualsAwakeNotesPatch(true);
        public override float OutlinesWidth { get; set; } = 3f;
        public override Visibility Visibility { get; set; } = Visibility.DesktopOnly;
        public override bool Glow { get; set; } = true;
        
        public bool ColorizeDebris { get; set; } = true;

        public CustomNotesPatch ColorizeCustomNotes = new(true);

    }
    
    public sealed partial class BombsConfig : ColorizeConfig {

        public override DynamicPatch Colorize { get; set; } = new ColorizeBombPatch(true);
        public DynamicPatch ColorizeOutlines { get; set; } = new BaseNoteVisualsAwakeBombsPatch(false);
        public override float OutlinesWidth { get; set; } = 5f;
        public override Visibility Visibility { get; set; } = Visibility.All;
        public override bool Glow { get; set; } = true;

    }
    
    public partial class ObstaclesConfig {

        public ObstacleControllerInitPatch Colorize { get; set; } = new(true);
        public bool Transparent { get; set; } = false;

    }

    public abstract class ColorizeConfig {

        public abstract DynamicPatch Colorize { get; set; }
        public abstract float OutlinesWidth { get; set; }
        public abstract Visibility Visibility { get; set; }
        public abstract bool Glow { get; set; }
        
    }
    
}