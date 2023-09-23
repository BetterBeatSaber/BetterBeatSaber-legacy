using BetterBeatSaber.Server.Services.Enums;
using BetterBeatSaber.Server.Services.Interfaces;
using BetterBeatSaber.Shared.Responses;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

using SteamWebAPI2.Interfaces;

using IPlayerService = BetterBeatSaber.Server.Services.Interfaces.IPlayerService;

namespace BetterBeatSaber.Server.Controllers; 

[Route("auth")]
[ApiController]
public sealed class AuthController : Controller {

    private readonly ISteamUserAuth _steamUserAuth;
    private readonly IPlayerService _playerService;
    private readonly ITokenService _tokenService;
    
    private readonly Network.Server.ServerOptions _serverOptions;
    
    public AuthController(ISteamUserAuth steamUserAuth, IPlayerService playerService, ITokenService tokenService, IOptionsMonitor<Network.Server.ServerOptions> serverOptions) {
        _steamUserAuth = steamUserAuth;
        _playerService = playerService;
        _tokenService = tokenService;
        _serverOptions = serverOptions.CurrentValue;
    }

    [EnableRateLimiting("authenticate")]
    [HttpPost]
    public async Task<ActionResult<AuthResponse>> Authenticate() {

        Request.EnableBuffering();
        Request.Body.Position = 0;

        using var streamReader = new StreamReader(HttpContext.Request.Body);

        var ticket = await streamReader.ReadToEndAsync();
        
        streamReader.Close();
        
        ulong steamId;
        try {
            
            var response = await _steamUserAuth.AuthenticateUserTicket(620980u, ticket);
            if(!response.Data.Response.Success || !ulong.TryParse(response.Data.Response.Params.SteamId, out steamId))
                return BadRequest("invalid Steam ticket");
            
        } catch (Exception) {
            return BadRequest("invalid Steam ticket");
        }
        
        var player = await _playerService.CreateOrUpdate(steamId);
        if (player == null)
            return BadRequest("failed to create or update player");

        if (player.IsBanned)
            return Forbid();

        return new AuthResponse {
            Token = _tokenService.CreateToken(player, TokenType.Session),
            Player = player.ToSharedModel(),
            Ip = _serverOptions.Ip,
            Port = _serverOptions.Port,
            Key = Network.Server.ConnectionKey
        };
        
    }

}