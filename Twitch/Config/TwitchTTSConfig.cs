using Newtonsoft.Json;

namespace BetterBeatSaber.Twitch.Config; 

public sealed class TwitchTTSConfig {

    #region Constants

    [JsonIgnore]
    public const string DefaultLanguage = "en-US";
    
    [JsonIgnore]
    public const string DefaultVoice = "en-US-GuyNeural";
    
    [JsonIgnore]
    public const int DefaultRate = 0;
    
    [JsonIgnore]
    public const int DefaultPitch = 0;

    [JsonIgnore]
    public const int DefaultVolume = 100;

    #endregion

    public string Language { get; set; } = DefaultLanguage;
    public string Voice { get; set; } = DefaultVoice;
    public int Rate { get; set; } = DefaultRate;
    public int Pitch { get; set; } = DefaultPitch;
    public int Volume { get; set; } = DefaultVolume;
    public bool Translate { get; set; } = false;
    public string OutputDevice { get; set; } = "00000000-0000-0000-0000-000000000000";

    public string[] IgnoredUserNames { get; set; } = {
        "Nightbot",
        "StreamElements",
        "Moobot",
        "discordstreamercommunity"
    };
    
}