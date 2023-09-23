using BetterBeatSaber.Core.Config;

namespace BetterBeatSaber.SmoothedControllers.Config; 

public sealed class SmoothedControllersConfig : Config<SmoothedControllersConfig> {

    public float PositionSmoothing { get; set; } = 3f;
    public float RotationSmoothing { get; set; } = 12f;
    public float SmallMovementThresholdAngle { get; set; } = 6f;

}