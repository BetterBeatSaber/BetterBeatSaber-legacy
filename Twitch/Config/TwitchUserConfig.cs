namespace BetterBeatSaber.Twitch.Config; 

public sealed class TwitchUserConfig {

    public string CustomName { get; set; } = null!;
    public string? CustomLanguage { get; set; }
    public string? CustomVoice { get; set; }
    public int? CustomRate { get; set; }
    public int? CustomPitch { get; set; }
    public int? CustomVolume { get; set; }

}