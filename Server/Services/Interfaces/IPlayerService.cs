using BetterBeatSaber.Shared.Enums;

using Player = BetterBeatSaber.Server.Models.Player;
using PlayerRelationship = BetterBeatSaber.Server.Models.PlayerRelationship;

namespace BetterBeatSaber.Server.Services.Interfaces; 

public interface IPlayerService {

    public IEnumerable<Player> Players { get; }
    
    public Task<Player?> CreateOrUpdate(ulong? steamId);
    
    public Task<Player?> GetFromHttpContext(HttpContext httpContext);
    
    public Task<Player?> GetById(ulong platformId);

    public Task<List<Player>> Search(string? name, int page, int count, bool banned);

    public Task<bool> DeletePlayer(Player player);
    
    public Task UpdateLeaderboardData();
    public Task UpdateLeaderboardData(Player player);
    
    //#region Integration
    //#endregion

    #region Relationship

    #region Friend

    public Task<List<Player>> GetFriends(Player player);
    public Task<List<Player>> GetFriendRequests(Player player);
    public Task<List<Player>> GetSentFriendRequests(Player player);

    public Task<bool> IsFriend(Player firstPlayer, Player secondPlayer);
    public Task<bool> HasFriendRequestFrom(Player player, Player otherPlayer);
    
    public Task SendFriendRequest(Player player, Player to);
    public Task<bool> AcceptFriendRequest(Player player, Player from);
    public Task<bool> DeclineFriendRequest(Player player, Player from);
    public Task<bool> WithdrawFriendRequest(Player player, Player to);

    public Task<bool> RemoveFriend(Player player, Player target);
    
    #endregion
    
    #region Block

    public Task<bool> HasBlocked(Player player, Player otherPlayer);
    
    public Task Block(Player player, Player otherPlayer);
    
    #endregion

    #endregion

}