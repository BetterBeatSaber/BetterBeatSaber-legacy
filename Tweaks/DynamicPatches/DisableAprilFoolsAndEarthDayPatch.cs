using System;

using BetterBeatSaber.Core.Harmomy.Dynamic;

namespace BetterBeatSaber.Tweaks.DynamicPatches; 

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

// https://github.com/ItsKaitlyn03/Unfunny/blob/main/Unfunny/HarmonyPatches/DateTime.cs

[DynamicPatch(typeof(DateTime), "get_Now")]
public sealed class DisableAprilFoolsAndEarthDayPatch : DynamicPatch {

    public DisableAprilFoolsAndEarthDayPatch(bool enabled) : base(enabled) { }
    
    private static void Postfix(ref DateTime __result) {
        if (__result is { Month: 4, Day: 1 or 22 })
            __result = __result.AddDays(1);
    }

}