using System.Net;

using BetterBeatSaber.Server.Services.Enums;
using BetterBeatSaber.Server.Services.Interfaces;
using BetterBeatSaber.Server.Steam;
using BetterBeatSaber.Shared.Responses;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

using IPlayerService = BetterBeatSaber.Server.Services.Interfaces.IPlayerService;

namespace BetterBeatSaber.Server.Controllers; 

[Route("auth")]
[ApiController]
public sealed class AuthController : Controller {

    private readonly Network.Server.ServerOptions _serverOptions;
    private readonly IPlayerService _playerService;
    private readonly ITokenService _tokenService;
    private readonly IBanService _banService;
    private readonly ISteamService _steamService;

    public AuthController(IOptionsMonitor<Network.Server.ServerOptions> serverOptions, IPlayerService playerService, ITokenService tokenService, IBanService banService, ISteamService steamService) {
        _serverOptions = serverOptions.CurrentValue;
        _playerService = playerService;
        _tokenService = tokenService;
        _banService = banService;
        _steamService = steamService;
    }

    [EnableRateLimiting("authenticate")]
    [HttpPost]
    public async Task<ActionResult<AuthResponse>> Authenticate() {

        Request.EnableBuffering();
        Request.Body.Position = 0;

        using var streamReader = new StreamReader(HttpContext.Request.Body);

        var ticket = await streamReader.ReadToEndAsync();
        
        var (authParams, authError) = await _steamService.Authenticate(620980u, ticket);
        if (authParams == null || authError != null) {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if(authError != null)
                return BadRequest(authError.ErrorDescription);
            return BadRequest("Invalid ticket");
        }
        
        if(!ulong.TryParse(authParams.SteamId, out var steamId))
            return BadRequest("Failed to parse Steam Id");
            
        if (await _banService.IsSteamBanned(steamId))
            return this.StatusCode((int) HttpStatusCode.Forbidden, "You have been banned from using this Mod");
        
        var player = await _playerService.CreateOrUpdate(steamId);
        if (player == null)
            return BadRequest("Failed to create or update player");

        return new AuthResponse {
            Token = _tokenService.CreateToken(player, TokenType.Session),
            Player = player.ToSharedModel(),
            Ip = _serverOptions.Ip,
            Port = _serverOptions.Port,
            Key = Network.Server.ConnectionKey
        };
        
    }

}