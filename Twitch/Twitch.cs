using BetterBeatSaber.Core;

namespace BetterBeatSaber.Twitch;

// ReSharper disable once UnusedType.Global
public sealed class Twitch : Module {

    public override void Init() {
        RegisterService<TwitchService>();
    }

    public override void Exit() {
        UnregisterService<TwitchService>();
    }

}