using BetterBeatSaber.Core.Config;
using BetterBeatSaber.Core.UI;

namespace BetterBeatSaber.Twitch.Chat.Config; 

public sealed class TwitchChatConfig : Config<TwitchChatConfig> {

    public FloatingViewConfigBase F { get; set; } = new();

}