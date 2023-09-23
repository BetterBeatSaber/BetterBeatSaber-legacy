using BetterBeatSaber.Core;

namespace BetterBeatSaber.ReeSabersColorizer;

// ReSharper disable once UnusedType.Global
public sealed class ReeSabersColorizer : Module {

    public override void Init() {
        Patch();
    }

    public override void Exit() {
        Unpatch();
    }

}