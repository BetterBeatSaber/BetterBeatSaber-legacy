using BetterBeatSaber.Server.Extensions;
using BetterBeatSaber.Server.Leaderboards.BeatLeader.Interfaces;
using BetterBeatSaber.Server.Leaderboards.ScoreSaber.Interfaces;
using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Steam.Models.SteamCommunity;

using SteamWebAPI2.Interfaces;

using IPlayerService = BetterBeatSaber.Server.Services.Interfaces.IPlayerService;

namespace BetterBeatSaber.Server.Controllers; 

[Route("players")]
[ApiController]
public sealed class PlayerController : Controller {

    private readonly IPlayerService _playerService;
    private readonly ISteamUser _steamUser;
    private readonly IBeatLeaderClient _beatLeaderClient;
    private readonly IScoreSaberClient _scoreSaberClient;

    public PlayerController(IPlayerService playerService, ISteamUser steamUser, IBeatLeaderClient beatLeaderClient, IScoreSaberClient scoreSaberClient) {
        _playerService = playerService;
        _steamUser = steamUser;
        _beatLeaderClient = beatLeaderClient;
        _scoreSaberClient = scoreSaberClient;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Player>> GetPlayer([FromRoute] ulong id) {

        var player = await _playerService.GetById(id);
        if (player != null)
            return player.ToSharedModel();

        PlayerSummaryModel? playerSummary;
        try {
            var response = await _steamUser.GetPlayerSummaryAsync(id);
            playerSummary = response.Data;
        } catch (Exception _) {
            return NotFound();
        }

        var scoreSaberPlayer = await _scoreSaberClient.GetPlayerInformation(id.ToString());
        var beatLeaderPlayer = await _beatLeaderClient.GetPlayer(id.ToString());
        
        return new Player {
            Id = id,
            Name = playerSummary.Nickname,
            AvatarUrl = playerSummary.AvatarFullUrl,
            Role = PlayerRole.None,
            Flags = 0,
            ScoreSaber = scoreSaberPlayer != null ? new Leaderboard {
                Country = scoreSaberPlayer.Country,
                Pp = scoreSaberPlayer.Pp,
                GlobalRank = scoreSaberPlayer.Rank,
                LocalRank = scoreSaberPlayer.CountryRank
            } : null,
            BeatLeader = beatLeaderPlayer != null ? new Leaderboard {
                Country = beatLeaderPlayer.Country,
                Pp = beatLeaderPlayer.Pp,
                GlobalRank = beatLeaderPlayer.Rank,
                LocalRank = beatLeaderPlayer.CountryRank
            } : null
        };

    }
    
    [HttpGet]
    public async Task<ActionResult<List<Player>>> Search([FromQuery] string? name = null, [FromQuery] int page = 0, [FromQuery] int count = 50) {

        if (name is { Length: < 2 })
            return BadRequest("Name too short");

        return await _playerService.Search(name, page, count).ToSharedModelList<Player, Models.Player>();

    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteMe() {
        
        var player = await _playerService.GetFromHttpContext(HttpContext);
        if (player == null)
            return Unauthorized();

        return await _playerService.DeletePlayer(player) ? Ok() : this.InternalError();

    }
    
}