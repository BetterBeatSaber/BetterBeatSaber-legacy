using BetterBeatSaber.Server.Extensions;
using BetterBeatSaber.Server.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Player = BetterBeatSaber.Shared.Models.Player;

namespace BetterBeatSaber.Server.Controllers; 

[Route("social")]
[ApiController]
[Authorize]
public sealed class SocialController : Controller {

    private readonly IPlayerService _playerService;

    public SocialController(IPlayerService playerService) {
        _playerService = playerService;
    }
    
    /// <summary>
    /// Get friends
    /// </summary>
    /// <returns>All friends</returns>
    [HttpGet("friends")]
    public async Task<ActionResult<List<Player>>> GetFriends() {
        
        var player = await _playerService.GetFromHttpContext(HttpContext);
        if (player == null)
            return Unauthorized();

        return await _playerService.GetFriends(player).ToSharedModelList<Player, Server.Models.Player>();
        
    }

    /// <summary>
    /// Get friend requests
    /// </summary>
    /// <returns>All friend requests</returns>
    [HttpGet("friends/requests")]
    public async Task<ActionResult<List<Player>>> GetFriendRequests() {

        var player = await _playerService.GetFromHttpContext(HttpContext);
        if (player == null)
            return Unauthorized();

        return await _playerService.GetFriendRequests(player).ToSharedModelList<Player, Server.Models.Player>();
        
    }
    
    /// <summary>
    /// Get sent friend requests
    /// </summary>
    /// <returns>All friend requests</returns>
    [HttpGet("friends/requests/sent")]
    public async Task<ActionResult<List<Player>>> GetSentFriendRequests() {

        var player = await _playerService.GetFromHttpContext(HttpContext);
        if (player == null)
            return Unauthorized();

        return await _playerService.GetSentFriendRequests(player).ToSharedModelList<Player, Server.Models.Player>();
        
    }

    /// <summary>
    /// Either sends a request or accepts a request depending if the other sent one
    /// </summary>
    /// <param name="id">The platform id of the other player</param>
    /// <returns>The state</returns>
    [HttpPost("friends/{id}")]
    public async Task<IActionResult> SendFriendRequestOrAcceptIt([FromRoute] ulong? id) {
        
        if (id == null)
            return BadRequest();
        
        var player = await _playerService.GetFromHttpContext(HttpContext);
        if (player == null)
            return Unauthorized();

        #if !DEBUG
        if (player.Id == id.Value)
            return BadRequest("You can't send yourself a friend request");
        #endif

        var target = await _playerService.GetById(id.Value);
        if (target == null)
            return NotFound();
        
        if (await _playerService.IsFriend(player, target))
            return BadRequest("already a friend");
        
        if (await _playerService.HasFriendRequestFrom(player, target)) {
            if (await _playerService.AcceptFriendRequest(player, target))
                return Ok();
            return StatusCode(418, "An error occurred ... it's your fault!");
        }

        if (await _playerService.HasFriendRequestFrom(target, player))
            return BadRequest("you already sent a request");

        await _playerService.SendFriendRequest(player, target);
        
        return Ok();

    }
    
    /// <summary>
    /// Either removes a friend or declines a request based if the target is a friend
    /// </summary>
    /// <param name="id">The platform id of the other player</param>
    /// <returns>The state</returns>
    [HttpDelete("friends/{id}")]
    public async Task<IActionResult> RemoveFriend([FromRoute] ulong? id) {

        if (id == null)
            return BadRequest();
        
        var player = await _playerService.GetFromHttpContext(HttpContext);
        if (player == null)
            return Unauthorized();

        var target = await _playerService.GetById(id.Value);
        if (target == null)
            return NotFound();
        
        if (await _playerService.IsFriend(player, target)) {
            if(await _playerService.RemoveFriend(player, target))
                return Ok("removed friend");
            return StatusCode(418, "An error occurred ... it's your fault!");
        }

        if (await _playerService.HasFriendRequestFrom(target, player)) {
            await _playerService.WithdrawFriendRequest(player, target);
            return Ok("request withdrawn");
        }

        if (!await _playerService.HasFriendRequestFrom(player, target))
            return BadRequest("no action available");
        
        await _playerService.DeclineFriendRequest(player, target);
        
        return Ok("declined request");

    }

    /*[HttpPost("block/{platformId}")]
    public async ActionResult<string> Block([FromRoute] ulong? platformId) {
        
        return Ok();
    }*/

}