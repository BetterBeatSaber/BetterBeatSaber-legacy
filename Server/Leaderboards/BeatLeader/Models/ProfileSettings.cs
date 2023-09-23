namespace BetterBeatSaber.Server.Leaderboards.BeatLeader.Models; 

#pragma warning disable CS8618

public class ProfileSettings {

    public int Id { get; set; }
    public string? Bio { get; set; }
    public string? Message { get; set; }
    public string? EffectName { get; set; }
    public string ProfileAppearance { get; set; }
    public float? Hue { get; set; }
    public float? Saturation { get; set; }
    public string? LeftSaberColor { get; set; }
    public string? RightSaberColor { get; set; }
    public object ProfileCover { get; set; }
    public string StarredFriends { get; set; }
    public bool ShowBots { get; set; }
    public bool ShowAllRatings { get; set; }
    
}