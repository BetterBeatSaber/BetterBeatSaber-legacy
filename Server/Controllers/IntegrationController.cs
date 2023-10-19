using BetterBeatSaber.Server.Models;
using BetterBeatSaber.Server.Services.Enums;
using BetterBeatSaber.Server.Services.Interfaces;
using BetterBeatSaber.Shared.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterBeatSaber.Server.Controllers; 

[Route("integrations")]
[ApiController]
public sealed class IntegrationController : Controller {

    private readonly IIntegrationService _integrationService;
    private readonly IPlayerService _playerService;
    private readonly ITokenService _tokenService;

    public IntegrationController(IIntegrationService integrationService, IPlayerService playerService, ITokenService tokenService) {
        this._integrationService = integrationService;
        this._playerService = playerService;
        this._tokenService = tokenService;
    }

    // TODO: Implement networking for live update and so UI for it lol

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerIntegration>>> GetAll() {
        
        var player = await _playerService.GetFromHttpContext(HttpContext);
        if (player == null)
            return Unauthorized();

        return Ok(await _integrationService.GetIntegrations(player));

    }

    [Authorize]
    [HttpGet("{integrationType}")]
    public async Task<ActionResult<PlayerIntegration>> Get([FromRoute] IntegrationType integrationType) {

        var player = await _playerService.GetFromHttpContext(HttpContext);
        if (player == null)
            return Unauthorized();
        
        var integration = await _integrationService.GetIntegration(player, integrationType);
        
        return integration != null ? integration : NotFound();
        
    }
    
    [Authorize]
    [HttpPost("{integrationType}")]
    public async Task<ActionResult> CreateToken([FromRoute] IntegrationType integrationType) {

        var player = await _playerService.GetFromHttpContext(HttpContext);
        if (player == null)
            return Unauthorized();

        return Ok(_tokenService.CreateToken(player, TokenType.Integration, integrationType.ToString(), TimeSpan.FromMinutes(20)));
        
    }

    [Authorize]
    [HttpDelete("{integrationType}")]
    public async Task<ActionResult> Delete([FromRoute] IntegrationType integrationType) {
        
        var player = await _playerService.GetFromHttpContext(HttpContext);
        if (player == null)
            return Unauthorized();

        return await _integrationService.RemoveIntegration(player, integrationType) ? Ok() : NotFound();

    }

    [HttpGet("add")]
    public async Task<ActionResult> Authorize([FromQuery] string token) {
        
        var type = _tokenService.VerifyTokenWithCustomData(token, TokenType.Integration);
        if (type == null || !Enum.TryParse(typeof(IntegrationType), type, out var integrationType))
            return BadRequest();

        var player = await _tokenService.GetPlayerFromToken(token);
        if(player == null)
            return BadRequest();

        var state = _tokenService.CreateToken(player, TokenType.IntegrationState, null, TimeSpan.FromMinutes(5));
        
        return Redirect(_integrationService.Integrations[(IntegrationType) integrationType].GetAuthorizationUrl(state));
        
    }

    [HttpGet("return/{integrationType}")]
    public async Task<IActionResult> Return([FromRoute] IntegrationType integrationType, [FromQuery] string? code, [FromQuery] string? state, [FromQuery] string? error, [FromQuery(Name = "error_description")] string? errorDescription, [FromQuery] string? scope) {

        var integration = _integrationService.Integrations[integrationType];
        
        if (error != null || state == null || code == null)
            return BadRequest($"Please try again: {error} - {errorDescription}");

        var player = await _tokenService.GetPlayerFromToken(state);
        if (player == null)
            BadRequest();

        var tokens = await integration.GetToken(code);
        if (tokens == null)
            return BadRequest();

        var playerIntegration = await _integrationService.AddOrUpdateIntegration(player, integrationType, tokens);
        
        return Ok(playerIntegration.Id.ToString());
        
    }

}