using System.ComponentModel.DataAnnotations;

using BetterBeatSaber.Server.Interfaces;
using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;

namespace BetterBeatSaber.Server.Models; 

#pragma warning disable CS8618

public sealed class PlayerIntegration : ISharedConvertable<Integration> {

    [Key]
    public Guid Id { get; init; }
    
    public Player Player { get; set; }
    
    public IntegrationType Type { get; set; }
    
    public byte[] AccessToken { get; set; } // Encrypted
    
    public byte[] RefreshToken { get; set; } // Encrypted
    
    public string TokenType { get; set; }
    
    public DateTime ExpiresAt { get; set; }

    public Integration ToSharedModel() => new() {
        Id = Id,
        Type = Type,
        Token = Player.DecryptToString(AccessToken),
        TokenType = TokenType,
        ExpiresAt = ExpiresAt
    };

}