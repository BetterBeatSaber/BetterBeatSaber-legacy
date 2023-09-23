using BetterBeatSaber.Core;
using BetterBeatSaber.Core.UI;
using BetterBeatSaber.Twitch.Chat.Config;
using BetterBeatSaber.Twitch.Chat.UI;

namespace BetterBeatSaber.Twitch.Chat;

public sealed class TwitchChat : Module {

    #region Init & Exit

    public override void Init() {
        
        Config = CreateConfig<TwitchChatConfig>();
        
        UIManager.Instance.RegisterFloatingView<ChatFloatingView>(TwitchChatConfig.Instance.F);
        
    }

    public override void Exit() {
        UIManager.Instance.UnregisterFloatingView<ChatFloatingView>();
    }

    #endregion

    #region Enable & Disable

    public override void Enable() {
    }

    public override void Disable() {
    }

    #endregion

}