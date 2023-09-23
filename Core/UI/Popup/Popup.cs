using BeatSaberMarkupLanguage.Attributes;

using JetBrains.Annotations;

using TMPro;

namespace BetterBeatSaber.Core.UI.Popup; 

#if DEBUG
[HotReload(RelativePathToLayout = "./Popup.bsml")]
#endif
public sealed class Popup : View<Popup> {

    [UsedImplicitly]
    [UIComponent(nameof(Title))]
    public readonly TMP_Text Title = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(Text))]
    public readonly TMP_Text Text = null!;

    public void SetTitle(string title) => Title.text = title;
    public void SetText(string text) => Text.text = text;

}