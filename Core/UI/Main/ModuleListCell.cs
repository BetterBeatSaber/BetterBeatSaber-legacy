using BeatSaberMarkupLanguage.Attributes;

using BetterBeatSaber.Core.TextMeshPro;
using BetterBeatSaber.Shared.Models;

using TMPro;

namespace BetterBeatSaber.Core.UI.Main; 

public sealed class ModuleListCell : ListCell<ModuleManifest> {

    [UIComponent(nameof(Name))]
    public readonly TMP_Text Name = null!;
    
    [UIComponent(nameof(Author))]
    public readonly TMP_Text Author = null!;
        
    public override void Populate(ModuleManifest moduleManifest) {
        
        Name.text = moduleManifest.Name;
        Author.text = "Made by " + moduleManifest.Author;
        
        TextMeshProAddon.Apply(Author);
        
    }
    
}