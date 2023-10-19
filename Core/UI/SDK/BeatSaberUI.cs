using BetterBeatSaber.Core.Utilities;

using Zenject;

namespace BetterBeatSaber.Core.UI.SDK; 

public sealed class BeatSaberUI : ConstructableSingleton<BeatSaberUI> {

    internal const int Layer = 5;
    
    public DiContainer? DiContainer { get; internal set; }
    
}