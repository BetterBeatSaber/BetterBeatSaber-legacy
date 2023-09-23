using BeatSaberMarkupLanguage.GameplaySetup;

namespace BetterBeatSaber.Core.UI; 

public abstract class ModifierView {

    public abstract string Title { get; }
    public virtual MenuType MenuType { get; } = MenuType.All;

}