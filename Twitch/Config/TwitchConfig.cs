using System.Collections.Generic;

using BetterBeatSaber.Core.Config;
using BetterBeatSaber.Core.UI;

namespace BetterBeatSaber.Twitch.Config; 

public partial class TwitchConfig : Config<TwitchConfig> {

    public FloatingViewConfigBase Chat { get; set; } = new();

    public TwitchTTSConfig TTS { get; set; } = new();
    
    public Dictionary<string, TwitchUserConfig> Users { get; set; } = new();

    public TwitchUserConfig GetUserConfig(string id) =>
        Users.TryGetValue(id, out var userConfig) ? userConfig : new TwitchUserConfig();
    
}