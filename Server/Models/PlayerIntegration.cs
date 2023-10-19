using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using BetterBeatSaber.Shared.Enums;

using Newtonsoft.Json;

namespace BetterBeatSaber.Server.Models; 

#pragma warning disable CS8618

public sealed class PlayerIntegration {

    [JsonIgnore]
    [Key]
    public Guid Id { get; init; }
    
    [JsonIgnore]
    public Player Player { get; set; }
    
    public IntegrationType Type { get; set; }

    [JsonIgnore]
    public byte[] AccessToken { get; set; } // Encrypted

    [NotMapped]
    [JsonProperty("access_token")]
    public string AccessTokenString => Player.DecryptToString(AccessToken);
    
    [JsonIgnore]
    public byte[] RefreshToken { get; set; } // Encrypted
    
    [JsonIgnore]
    public string TokenType { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    
}