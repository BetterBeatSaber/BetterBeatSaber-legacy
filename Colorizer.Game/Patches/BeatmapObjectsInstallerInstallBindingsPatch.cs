using BetterBeatSaber.Colorizer.Game.Config;

using HarmonyLib;

using IPA.Utilities;

using UnityEngine;

namespace BetterBeatSaber.Colorizer.Game.Patches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

[HarmonyPatch(typeof(BeatmapObjectsInstaller), nameof(BeatmapObjectsInstaller.InstallBindings), MethodType.Normal)]
internal static class BeatmapObjectsInstallerInstallBindingsPatch {

    private static readonly FieldAccessor<ObstacleController, GameObject[]>.Accessor ObstacleControllerVisualWrappersAccessor = FieldAccessor<ObstacleController, GameObject[]>.GetAccessor("_visualWrappers");
    
    private static GameObject[]? _visualWrappersOriginal;
    
    // ReSharper disable once InconsistentNaming
    [HarmonyPostfix]
    [HarmonyPriority(int.MaxValue)]
    private static void Postfix(ObstacleController ____obstaclePrefab) {
        
        if(_visualWrappersOriginal != null) {
            if(GameColorizerConfig.Instance.Obstacles.Transparent)
                return;
            ObstacleControllerVisualWrappersAccessor(ref ____obstaclePrefab) = _visualWrappersOriginal;
            _visualWrappersOriginal = null;
            return;
        }

        if(!GameColorizerConfig.Instance.Obstacles.Transparent)
            return;

        _visualWrappersOriginal = ObstacleControllerVisualWrappersAccessor(ref ____obstaclePrefab);
        if(_visualWrappersOriginal.Length != 2)
            return;

        ObstacleControllerVisualWrappersAccessor(ref ____obstaclePrefab) = new[] { _visualWrappersOriginal[1] };

        _visualWrappersOriginal[0].SetActive(false);
        
    }

}