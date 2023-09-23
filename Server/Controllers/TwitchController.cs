using BetterBeatSaber.Server.Services.Enums;
using BetterBeatSaber.Server.Services.Interfaces;
using BetterBeatSaber.Shared.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterBeatSaber.Server.Controllers; 

[Route("twitch")]
[ApiController]
public sealed class TwitchController : Controller {

    private readonly IAzureService _azureService;
    private readonly IPlayerService _playerService;
    private readonly ITokenService _tokenService;

    public TwitchController(IAzureService azureService, IPlayerService playerService, ITokenService tokenService) {
        _azureService = azureService;
        _playerService = playerService;
        _tokenService = tokenService;
    }
    
    [Authorize]
    [HttpPost("tts")]
    public async Task<IActionResult> Speak([FromQuery] string token) {

        var player = await _playerService.GetFromHttpContext(HttpContext);
        if (player == null)
            return Unauthorized();

        if (player.Role < PlayerRole.Supporter)
            return Forbid();

        var ssml = _tokenService.VerifyTokenWithCustomData(token, TokenType.TwitchTTS);
        if (ssml == null)
            return Forbid();
            
        var audioData = await _azureService.Speak(ssml);
        if (audioData == null)
            return BadRequest();

        return new FileContentResult(audioData, "audio/pcm");

    }

}