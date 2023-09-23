namespace BetterBeatSaber.Server.Leaderboards.ScoreSaber.Models; 

public class ScoreStats {

    public ulong TotalScore { get; set; }
    public ulong TotalRankedScore { get; set; }
    public float AverageRankedAccuracy { get; set; }
    public uint TotalPlayCount { get; set; }
    public uint RankedPlayCount { get; set; }
    public uint ReplaysWatched { get; set; }

}