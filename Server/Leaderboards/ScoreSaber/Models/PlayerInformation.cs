namespace BetterBeatSaber.Server.Leaderboards.ScoreSaber.Models; 

#pragma warning disable CS8618

public class PlayerInformation {

    public string Id { get; set; }
    public string Name { get; set; }
    public string ProfilePicture { get; set; }
    public string Bio { get; set; }
    public string Country { get; set; }
    public float Pp { get; set; }
    public uint Rank { get; set; }
    public uint CountryRank { get; set; }
    public string? Role { get; set; }
    public IEnumerable<Badge>? Badges { get; set; }
    public string Histories { get; set; }
    public int Permissions { get; set; }
    public bool Banned { get; set; }
    public bool Inactive { get; set; }
    public ScoreStats? ScoreStats { get; set; }

}