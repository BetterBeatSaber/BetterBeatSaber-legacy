namespace BetterBeatSaber.Server.Leaderboards.BeatLeader.Models; 

#pragma warning disable CS8618

public sealed class PlayerList {

    public PlayerListMetadata Metadata { get; set; }
    public IEnumerable<Player> Data { get; set; }

}

public sealed class PlayerListMetadata {

    public uint ItemsPerPage { get; set; }
    public uint Page { get; set; }
    public uint Total { get; set; }

}