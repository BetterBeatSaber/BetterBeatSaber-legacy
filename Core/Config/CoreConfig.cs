using System.Collections.Generic;

using BetterBeatSaber.Core.UI;

namespace BetterBeatSaber.Core.Config; 

#pragma warning disable CS8618

public sealed class CoreConfig : Config<CoreConfig> {

    public List<string> Modules { get; set; } = new();
    
    public bool CustomTitle { get; set; } = true;
    
    public FloatingViewConfigBase FriendScreen { get; set; } = new();

}