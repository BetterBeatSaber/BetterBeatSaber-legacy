using System.Security.Claims;

using BetterBeatSaber.Server.Leaderboards;
using BetterBeatSaber.Server.Network.Interfaces;
using BetterBeatSaber.Server.Services.Interfaces;
using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Network.Packets;

using Microsoft.EntityFrameworkCore;

using Steam.Models.SteamCommunity;

using SteamWebAPI2.Interfaces;

using IPlayerService = BetterBeatSaber.Server.Services.Interfaces.IPlayerService;
using Player = BetterBeatSaber.Server.Models.Player;
using PlayerRelationship = BetterBeatSaber.Server.Models.PlayerRelationship;

namespace BetterBeatSaber.Server.Services; 

public sealed class PlayerService : IPlayerService {

    private readonly AppContext _context;
    private readonly IServer _server;
    private readonly ISteamUser _steamUser;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IConfigService _configService;

    public IEnumerable<Player> Players => _context.Players;
    
    public PlayerService(AppContext context, IServer server, ISteamUser steamUser, IServiceScopeFactory serviceScopeFactory, IConfigService configService) {
        _context = context;
        _server = server;
        _steamUser = steamUser;
        _serviceScopeFactory = serviceScopeFactory;
        _configService = configService;
    }

    public async Task<Player?> CreateOrUpdate(ulong? steamId) {

        if (steamId == null)
            return null;

        PlayerSummaryModel playerSummary;
        try {
            var response = await _steamUser.GetPlayerSummaryAsync(steamId.Value);
            playerSummary = response.Data;
        } catch (Exception) {
            return null;
        }
        
        var player = await _context.Players.FirstOrDefaultAsync(player => player.Id == steamId.Value);
        if (player != null) {

            if(!player.Flags.HasFlag(PlayerFlag.HasCustomName))
                player.Name = playerSummary.Nickname;

            player.AvatarUrl = playerSummary.AvatarFullUrl;
            
            player.LastUpdate = DateTime.Now;

        } else {
            
            player = new Player {
                Id = steamId.Value,
                Name = playerSummary.Nickname,
                AvatarUrl = playerSummary.AvatarFullUrl,
                Role = PlayerRole.Player,
                IsBanned = false,
                LastUpdate = DateTime.Now
            };

            await _context.Players.AddAsync(player);
            
        }

        await _context.SaveChangesAsync();
        
        return player;

    }

    public async Task<Player?> GetFromHttpContext(HttpContext httpContext) {
        
        // ReSharper disable once HeapView.ClosureAllocation
        var id = httpContext.User.FindFirstValue(TokenService.JwtIdClaimName);
        if (id == null)
            return null;
        
        return await _context.Players.Where(player => player.Id.ToString() == id).FirstOrDefaultAsync();
        
    }

    public async Task<Player?> GetById(ulong platformId) {
        return await _context.Players.FirstOrDefaultAsync(player => player.Id == platformId);
    }

    public async Task<List<Player>> Search(string? name, int page = 0, int count = 50, bool banned = false) {
        
        var query = _context.Players.AsQueryable();
        if (name != null)
            query = query.Where(p => p.Name.Contains(name) || p.Name == name);
        
        if (banned)
            query = query.Where(p => p.IsBanned);
        
        return await query.Skip(page * count)
                          .Take(count)
                          .ToListAsync();
        
    }

    public async Task<bool> DeletePlayer(Player player) {
        try {
            
            foreach (var integration in _context.PlayerIntegrations.Where(integration => integration.Player == player))
                _context.PlayerIntegrations.Remove(integration);

            foreach (var relationship in _context.PlayerRelationships.Where(relationship => relationship.FirstPlayer == player || relationship.SecondPlayer == player))
                _context.PlayerRelationships.Remove(relationship);

            await _configService.DeleteAllConfigs(player);
            
            _context.Players.Remove(player);

            await _context.SaveChangesAsync();
            
            return true;
            
        } catch (Exception) {
            return false;
        }
    }

    #region Leaderboard

    public async Task UpdateLeaderboardData() {
        foreach (var player in await _context.Players.ToListAsync())
            await UpdateLeaderboardData(player);
    }

    public async Task UpdateLeaderboardData(Player player) {

        using var scope = _serviceScopeFactory.CreateScope();

        foreach (var client in scope.ServiceProvider.GetServices<IBaseLeaderboardClient>())
            await client.UpdatePlayer(player);

        await _context.SaveChangesAsync();

    }

    #endregion

    //#region Integration
    //#endregion

    #region Relationship

    #region Friend

    public async Task<List<Player>> GetFriends(Player player) {
        return await _context
                     .PlayerRelationships
                     .Where(relationship => relationship.RelationshipType == RelationshipType.Friend && (relationship.FirstPlayer == player || relationship.SecondPlayer == player))
                     .Select(playerRelationship => playerRelationship.FirstPlayer == player ? playerRelationship.SecondPlayer : playerRelationship.FirstPlayer)
                     .ToListAsync();
    }

    public async Task<List<Player>> GetFriendRequests(Player player) {
        return await _context
                     .PlayerRelationships
                     .Where(relationship => relationship.RelationshipType == RelationshipType.Request && relationship.SecondPlayer == player)
                     .Select(playerRelationship => playerRelationship.FirstPlayer)
                     .ToListAsync();
    }

    public async Task<List<Player>> GetSentFriendRequests(Player player) {
        return await _context
                     .PlayerRelationships
                     .Where(relationship => relationship.RelationshipType == RelationshipType.Request && relationship.FirstPlayer == player)
                     .Select(playerRelationship => playerRelationship.SecondPlayer)
                     .ToListAsync();
    }

    public async Task<bool> IsFriend(Player firstPlayer, Player secondPlayer) {
        return await _context
            .PlayerRelationships
            .AnyAsync(relationship => relationship.RelationshipType == RelationshipType.Friend && ((relationship.FirstPlayer == firstPlayer && relationship.SecondPlayer == secondPlayer) || (relationship.FirstPlayer == secondPlayer && relationship.SecondPlayer == firstPlayer)));
    }

    public async Task<bool> HasFriendRequestFrom(Player player, Player otherPlayer) {
        return await _context
            .PlayerRelationships
            .AnyAsync(relationship => relationship.RelationshipType == RelationshipType.Request && relationship.FirstPlayer == otherPlayer && relationship.SecondPlayer == player);
    }

    public async Task SendFriendRequest(Player player, Player to) {

        var relationship = new PlayerRelationship {
            Id = Guid.NewGuid(),
            FirstPlayer = player,
            SecondPlayer = to,
            RelationshipType = RelationshipType.Request,
            UpdatedAt = DateTime.Now,
            CreatedAt = DateTime.Now
        };

        await _context.PlayerRelationships.AddAsync(relationship);
        await _context.SaveChangesAsync();
        
        _server.SendPacketToPlayerIfConnected(to, new FriendRelationshipPacket {
            Relationship = FriendRelationship.RequestReceived,
            Player = player.ToSharedModel()
        });

    }

    public async Task<bool> AcceptFriendRequest(Player player, Player from) {

        var relationship = await GetFriendRequest(player, from);
        if (relationship == null)
            return false;
        
        relationship.RelationshipType = RelationshipType.Friend;
        relationship.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        _server.SendPacketToPlayerIfConnected(from, new FriendRelationshipPacket {
            Relationship = FriendRelationship.RequestAccepted,
            Player = player.ToSharedModel()
        });
        
        return true;

    }

    public async Task<bool> DeclineFriendRequest(Player player, Player from) {
        
        var relationship = await GetFriendRequest(player, from);
        if (relationship == null)
            return false;

        _context.PlayerRelationships.Remove(relationship);
        
        await _context.SaveChangesAsync();
        
        _server.SendPacketToPlayerIfConnected(from, new FriendRelationshipPacket {
            Relationship = FriendRelationship.RequestDeclined,
            Player = player.ToSharedModel()
        });

        return true;
        
    }
    
    public async Task<bool> WithdrawFriendRequest(Player player, Player to) {
        
        var relationship = await GetFriendRequest(to, player);
        if (relationship == null)
            return false;

        _context.PlayerRelationships.Remove(relationship);
        
        await _context.SaveChangesAsync();
        
        _server.SendPacketToPlayerIfConnected(to, new FriendRelationshipPacket {
            Relationship = FriendRelationship.RequestWithdrawn,
            Player = player.ToSharedModel()
        });

        return true;
        
    }

    public async Task<bool> RemoveFriend(Player player, Player target) {

        var relationship = await GetFriend(player, target);
        if (relationship == null)
            return false;

        _context.PlayerRelationships.Remove(relationship);

        await _context.SaveChangesAsync();
        
        _server.SendPacketToPlayerIfConnected(target, new FriendRelationshipPacket {
            Relationship = FriendRelationship.FriendRemoved,
            Player = player.ToSharedModel()
        });

        return true;

    }

    #region Private

    private async Task<PlayerRelationship?> GetFriend(Player firstPlayer, Player secondPlayer) {
        return await _context
                     .PlayerRelationships
                     .FirstOrDefaultAsync(relationship => relationship.RelationshipType == RelationshipType.Friend && ((relationship.FirstPlayer == firstPlayer && relationship.SecondPlayer == secondPlayer) || (relationship.FirstPlayer == secondPlayer && relationship.SecondPlayer == firstPlayer)));
    }
    
    private async Task<PlayerRelationship?> GetFriendRequest(Player player, Player from) {
        return await _context
            .PlayerRelationships
            .FirstOrDefaultAsync(relationship => relationship.RelationshipType == RelationshipType.Request && relationship.FirstPlayer == from && relationship.SecondPlayer == player);
    }

    #endregion

    #endregion
    
    #region Block
    
    public async Task<bool> HasBlocked(Player player, Player otherPlayer) {
        return await _context
            .PlayerRelationships
            .AnyAsync(relationship => relationship.RelationshipType == RelationshipType.Block && relationship.FirstPlayer == player && relationship.SecondPlayer == otherPlayer);
    }

    public async Task Block(Player player, Player otherPlayer) {

        var relationship = new PlayerRelationship {
            Id = Guid.NewGuid(),
            FirstPlayer = player,
            SecondPlayer = otherPlayer,
            RelationshipType = RelationshipType.Block,
            UpdatedAt = DateTime.Now,
            CreatedAt = DateTime.Now
        };

        await _context.PlayerRelationships.AddAsync(relationship);
        await _context.SaveChangesAsync();

    }

    #endregion
    
    #endregion

}