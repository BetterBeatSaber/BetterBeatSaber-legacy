using BetterBeatSaber.Core;

namespace BetterBeatSaber.MissTexts;

// ReSharper disable once UnusedType.Global
public sealed class MissTexts : Module {

    public override void Enable() {
        Patch();
    }

    public override void Disable() {
        Unpatch();
    }

}