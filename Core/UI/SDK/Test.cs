using BetterBeatSaber.Core.UI.SDK.Attributes;
using BetterBeatSaber.Core.UI.SDK.Components;

using JetBrains.Annotations;

namespace BetterBeatSaber.Core.UI.SDK; 

public class Test : NewView {

    [UsedImplicitly]
    [Component(nameof(Text))]
    public Text Text { get; set; } = null!; 
    
}