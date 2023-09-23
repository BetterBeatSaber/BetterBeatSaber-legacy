using BetterBeatSaber.Colorizer.Game.Colorizer;
using BetterBeatSaber.Core.Harmomy.Dynamic;

using HarmonyLib;

namespace BetterBeatSaber.Colorizer.Game.DynamicPatches; 

[DynamicPatch(typeof(ObstacleController), nameof(ObstacleController.Init))]
public sealed class ObstacleControllerInitPatch : DynamicPatch {

    public ObstacleControllerInitPatch(bool enabled) : base(enabled) { }
    
    [HarmonyPostfix]
    [HarmonyPriority(int.MaxValue)]
    public static void Postfix(ObstacleController __instance) {
        __instance.gameObject.AddComponent<ObstacleColorizer>();
    }

}