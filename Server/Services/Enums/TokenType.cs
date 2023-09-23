namespace BetterBeatSaber.Server.Services.Enums; 

[Flags]
public enum TokenType : byte {

    All = Session | Integration | IntegrationState | TwitchTTS,
    
    Session = 0x02,
    Integration = 0x04,
    IntegrationState = 0x08,
    TwitchTTS = 0x16

}