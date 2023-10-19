using System.ComponentModel.DataAnnotations;

namespace BetterBeatSaber.Server.Models; 

public sealed class TwitchUser {

    [Key]
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Language { get; set; } = null!;

}