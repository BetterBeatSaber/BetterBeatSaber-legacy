using System;

namespace BetterBeatSaber.Twitch.Shared.Enums; 

[Flags]
public enum FeatureFlag : ushort {

    None = 0,
    
    // Default Twitch
    Chat = 0x0200,
    Follows = 0x0201,
    Subs = 0x0202,
    Raids = 0x0203,
    ChannelPoints = 0x0204,
    
    // Custom advanced
    Requests = 0x0301, // Requires Chat
    
    TextToSpeech = 0x0302, // Requires Chat
    AutoTranslation = 0x0303 // Requires Chat

}