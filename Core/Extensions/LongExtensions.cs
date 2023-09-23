using System;

namespace BetterBeatSaber.Core.Extensions; 

public static class LongExtensions {

    private const int Unit = 1024;
    private const string UnitString = "B";

    public static string ToBytesCount(this long bytes) {
        if (bytes < Unit)
            return $"{bytes} {UnitString}";
        var exp = (int) (Math.Log(bytes) / Math.Log(Unit));
        // ReSharper disable once StringLiteralTypo
        return $"{bytes / Math.Pow(Unit, exp):##.##} {"KMGTPEZY"[exp - 1]}{UnitString}";
    }

}