using System;

using BetterBeatSaber.Shared.Enums;

namespace BetterBeatSaber.Shared.Models; 

public partial class ModuleManifest {

    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Version { get; set; } = null!;
    public string? Author { get; set; } = null!;
    public string? Description { get; set; } = null!;

    public string[]? Dependencies { get; set; } = Array.Empty<string>();
    public string[]? ConflictsWith { get; set; } = Array.Empty<string>();
    public string? MinGameVersion { get; set; }
    
    public bool? RequiresSoftRestart { get; set; } = false;
    public bool? RequiresFullRestart { get; set; } = false;

    public PlayerRole? RequiredRole { get; set; } = PlayerRole.Player;

}