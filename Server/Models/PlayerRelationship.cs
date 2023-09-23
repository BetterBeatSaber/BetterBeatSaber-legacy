using System.ComponentModel.DataAnnotations;

using BetterBeatSaber.Shared.Enums;

namespace BetterBeatSaber.Server.Models; 

#pragma warning disable CS8618

public sealed class PlayerRelationship {

    [Key]
    public Guid Id { get; init; }
    
    public Player FirstPlayer { get; set; }
    
    public Player SecondPlayer { get; set; }
    
    public RelationshipType RelationshipType { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public DateTime CreatedAt { get; set; }

}