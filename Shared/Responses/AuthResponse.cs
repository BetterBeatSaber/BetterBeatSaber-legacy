using BetterBeatSaber.Shared.Models;

namespace BetterBeatSaber.Shared.Responses; 

#pragma warning disable CS8618

public sealed class AuthResponse {

    public string Token { get; set; }
    
    public Player Player { get; set; }
    
    public string Ip { get; set; }
    public ushort Port { get; set; }
    public string Key { get; set; }
    
}