using BetterBeatSaber.Core;
using BetterBeatSaber.SmoothedControllers.Config;

namespace BetterBeatSaber.SmoothedControllers;

// ReSharper disable once UnusedType.Global
public sealed class SmoothedControllers : Module {

    public override void Init() {
        CreateConfig<SmoothedControllersConfig>();
        Patch();
    }

    public override void Exit() {
        Unpatch();
    }

}