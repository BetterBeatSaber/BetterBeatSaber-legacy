using BetterBeatSaber.Colorizer.Game.Config;
using BetterBeatSaber.Colorizer.Game.Utilities;
using BetterBeatSaber.Core.Harmomy.Dynamic;

namespace BetterBeatSaber.Colorizer.Game.DynamicPatches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

[DynamicPatch(typeof(BaseNoteVisuals), "Awake")]
public sealed class BaseNoteVisualsAwakeNotesPatch : DynamicPatch {
    
    public BaseNoteVisualsAwakeNotesPatch(bool enabled) : base(enabled) { }

    public static void Postfix(NoteController ____noteController) {
        if (____noteController is GameNoteController) {
            Colorize(____noteController, GameColorizerConfig.Instance.Notes);
        }
    }

    internal static void Colorize(NoteController noteController, GameColorizerConfig.ColorizeConfig colorizeConfig) {
        var outline = noteController.gameObject.GetComponent<Outline>();
        if(outline == null)
            outline = noteController.gameObject.AddComponent<Outline>();
        outline.OutlineWidth = colorizeConfig.OutlinesWidth;
        outline.Visibility = colorizeConfig.Visibility;
        outline.Glowing = colorizeConfig.Glow;
    }

}

[DynamicPatch(typeof(BaseNoteVisuals), "Awake")]
public sealed class BaseNoteVisualsAwakeBombsPatch : DynamicPatch {

    public BaseNoteVisualsAwakeBombsPatch(bool enabled) : base(enabled) { }

    public static void Postfix(NoteController ____noteController) {
        if (____noteController is BombNoteController) {
            BaseNoteVisualsAwakeNotesPatch.Colorize(____noteController, GameColorizerConfig.Instance.Bombs);
        }
    }

}