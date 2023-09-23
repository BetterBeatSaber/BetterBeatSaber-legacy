using System.Reflection;

using BetterBeatSaber.Colorizer.Game.Config;
using BetterBeatSaber.Colorizer.Game.Utilities;
using BetterBeatSaber.Core.Harmomy.Dynamic;

using IPA.Loader;

using UnityEngine;

namespace BetterBeatSaber.Colorizer.Game.DynamicPatches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

public sealed class CustomNotesPatch : DynamicPatch {

    public CustomNotesPatch(bool enabled) : base(enabled) { }

    protected override MethodBase? TargetMethod => PluginManager.GetPluginFromId("Custom Notes")?.Assembly.GetType("CustomNotes.Managers.CustomNoteController")?.GetMethod("Visuals_DidInit", DefaultBindingFlags);

    public static void Postfix(GameObject ___activeNote) {
        
        var outline = ___activeNote.gameObject.GetComponent<Outline>();
        if(outline == null)
            outline = ___activeNote.gameObject.AddComponent<Outline>();
        
        outline.OutlineWidth = GameColorizerConfig.Instance.Notes.OutlinesWidth;
        outline.Visibility = GameColorizerConfig.Instance.Notes.Visibility;
        outline.Glowing = GameColorizerConfig.Instance.Notes.Glow;
        
    }

}