namespace BetterBeatSaber.Server.Services.Enums; 

[Flags]
public enum TokenType : byte {

    All = Session | Integration | IntegrationState | TwitchTextToSpeech,
    
    Session = 0x02,
    Integration = 0x04,
    IntegrationState = 0x08,
    TwitchTextToSpeech = 0x16

}