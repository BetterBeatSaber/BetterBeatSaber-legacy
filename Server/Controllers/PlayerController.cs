using BetterBeatSaber.Server.Extensions;
using BetterBeatSaber.Server.Services.Interfaces;
using BetterBeatSaber.Shared.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterBeatSaber.Server.Controllers; 

[Route("players")]
[ApiController]
public sealed class PlayerController : Controller {

    private readonly IPlayerService _playerService;

    public PlayerController(IPlayerService playerService) {
        _playerService = playerService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Player>> GetPlayer([FromRoute] ulong id) {

        var player = await _playerService.GetById(id);
        if (player != null)
            return player.ToSharedModel();

        // TODO: To have profiles everywhere construct a "player" by getting their data
        return NotFound();

    }
    
    [HttpGet]
    public async Task<ActionResult<List<Player>>> Search([FromQuery] string? name = null, [FromQuery] int page = 0, [FromQuery] int count = 50, [FromQuery] bool banned = false) {

        if (name is { Length: < 2 })
            return BadRequest("Name too short");

        return await _playerService.Search(name, page, count, banned).ToSharedModelList<Player, Models.Player>();

    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<Player>> GetMe() {
        
        var player = await _playerService.GetFromHttpContext(HttpContext);
        if (player == null)
            return Unauthorized();

        return player.ToSharedModel();

    }

    [Authorize]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe() {
        
        var player = await _playerService.GetFromHttpContext(HttpContext);
        if (player == null)
            return Unauthorized();

        return await _playerService.DeletePlayer(player) ? Ok() : this.InternalError();

    }
    
}