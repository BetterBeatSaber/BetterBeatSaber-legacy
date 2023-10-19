using System.ComponentModel.DataAnnotations;

namespace BetterBeatSaber.Server.Models; 

public sealed class Ban {

    [Key]
    public Guid Id { get; set; }
    
    public ulong? SteamId { get; set; }
    public ulong? DiscordId { get; set; }
    public ulong? BeatSaverId { get; set; }
    
}