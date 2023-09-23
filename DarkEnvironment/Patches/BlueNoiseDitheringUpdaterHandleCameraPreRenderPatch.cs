using BetterBeatSaber.DarkEnvironment.Config;

using HarmonyLib;

using UnityEngine;

namespace BetterBeatSaber.DarkEnvironment.Patches; 

[HarmonyPatch(typeof(BlueNoiseDitheringUpdater), nameof(BlueNoiseDitheringUpdater.HandleCameraPreRender), MethodType.Normal)]
public sealed class BlueNoiseDitheringUpdaterHandleCameraPreRenderPatch {

    private static readonly int GlobalNoiseTextureID = Shader.PropertyToID("_GlobalBlueNoiseTex");
    private static bool _lastDisableState;

    [HarmonyPrefix]
    [HarmonyPriority(int.MaxValue)]
    public static bool Prefix() {
        
        if(!DarkEnvironmentConfig.Instance.DisableMenuCameraNoise) {
            _lastDisableState = false;
            return true;
        }

        if (_lastDisableState)
            return false;
        
        Shader.SetGlobalTexture(GlobalNoiseTextureID, null);
        _lastDisableState = true;

        return false;
        
    }

}