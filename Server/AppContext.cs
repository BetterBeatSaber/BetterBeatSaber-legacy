using BetterBeatSaber.Server.Models;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

namespace BetterBeatSaber.Server; 

#pragma warning disable CS8618

public sealed class AppContext : DbContext {

    public AppContext(DbContextOptions options) : base(options) { }
    
    [UsedImplicitly]
    public DbSet<Player> Players { get; set; }
    
    [UsedImplicitly]
    public DbSet<PlayerIntegration> PlayerIntegrations { get; set; }
 
    [UsedImplicitly]
    public DbSet<PlayerRelationship> PlayerRelationships { get; set; }
 
    [UsedImplicitly]
    public DbSet<Ban> Bans { get; set; }
    
}