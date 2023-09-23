using System;

using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Network.Enums;

namespace BetterBeatSaber.Core.Extensions; 

public static class BeatmapExtensions {

    private const string CustomLevelPrefix = "custom_level_";
    
    public static bool IsCustomLevel(this IDifficultyBeatmap difficultyBeatmap) =>
        difficultyBeatmap.level.levelID.StartsWith(CustomLevelPrefix);

    public static bool IsCustomLevel(this IPreviewBeatmapLevel difficultyBeatmap) =>
        difficultyBeatmap.levelID.StartsWith(CustomLevelPrefix);
    
    public static string GetHash(this IDifficultyBeatmap difficultyBeatmap) =>
        difficultyBeatmap.level.levelID.Substring(CustomLevelPrefix.Length).ToLower();

    public static string GetHash(this IPreviewBeatmapLevel difficultyBeatmap) =>
        difficultyBeatmap.levelID.Substring(CustomLevelPrefix.Length).ToLower();
    
    public static MapType GetMapType(this IDifficultyBeatmap difficultyBeatmap) =>
        difficultyBeatmap.IsCustomLevel() ? MapType.Custom : MapType.Default;
    
    public static MapDifficulty GetMapDifficulty(this IDifficultyBeatmap difficultyBeatmap) =>
        (MapDifficulty) Enum.Parse(typeof(MapDifficulty), difficultyBeatmap.difficulty.ToString());

}