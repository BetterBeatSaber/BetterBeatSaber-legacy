using System;
using System.Reflection;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Utilities;

using HarmonyLib;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

namespace BetterBeatSaber.Core.Bindings; 

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantAssignment

public sealed class FireworksBinding : IInitializable, IDisposable {

    [UsedImplicitly]
    [Inject]
    private readonly FireworksController _fireworksController = null!;

    private MethodInfo? _originalFireCoroutineMethod;
    private MethodInfo? _originalAwakeMethod;
    
    public void Initialize() {

        _originalFireCoroutineMethod = typeof(FireworkItemController).GetMethod(nameof(FireworkItemController.FireCoroutine), BindingFlags.Public | BindingFlags.Instance);
        _originalAwakeMethod = typeof(FireworkItemController).GetMethod(nameof(FireworkItemController.Awake), BindingFlags.Public | BindingFlags.Instance);
        
        BetterBeatSaber.Harmony.Patch(_originalFireCoroutineMethod, new HarmonyMethod(typeof(FireworksBinding).GetMethod(nameof(FireCoroutinePrefix), BindingFlags.NonPublic | BindingFlags.Static)));
        BetterBeatSaber.Harmony.Patch(_originalAwakeMethod, postfix: new HarmonyMethod(typeof(FireworksBinding).GetMethod(nameof(AwakePostfix), BindingFlags.NonPublic | BindingFlags.Static)));
       
        _fireworksController.SetField("_spawnSize", new Vector3(50f, 5f, 50f));
        _fireworksController.SetField("_minSpawnInterval", .001f);
        _fireworksController.SetField("_maxSpawnInterval", .05f);
        _fireworksController.SetField("_lightsIntensity", 5f);
        
        _fireworksController.enabled = true;
        
    }
        
    public void Dispose() {
        BetterBeatSaber.Harmony.Unpatch(_originalFireCoroutineMethod, HarmonyPatchType.Prefix);
        BetterBeatSaber.Harmony.Unpatch(_originalAwakeMethod, HarmonyPatchType.Postfix);
    }

    private static void FireCoroutinePrefix(ref FireworkItemController __instance, ref bool ____randomizeColor) {
        ____randomizeColor = true;
        __instance.SetField("_lightsColorGradient", new Gradient {
            colorKeys = new GradientColorKey[] {
                new(RGB.Color0, 0f),
                new(RGB.Color1, 1f)
            }
        });
    }

    private static void AwakePostfix(ref AudioSource ____audioSource) {
        ____audioSource.volume = .0005f;
    }

}