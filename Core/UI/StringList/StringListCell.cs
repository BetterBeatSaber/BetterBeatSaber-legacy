using BeatSaberMarkupLanguage.Attributes;

using JetBrains.Annotations;

using TMPro;

namespace BetterBeatSaber.Core.UI.StringList; 

public class StringListCell : ListCell<string> {

    [UsedImplicitly]
    [UIAction(nameof(Text))]
    public readonly TMP_Text Text = null!;
        
    public override void Populate(string text) {
        Text.text = text;
    }

}