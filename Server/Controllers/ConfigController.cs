using System.Text;

using BetterBeatSaber.Server.Extensions;
using BetterBeatSaber.Server.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BetterBeatSaber.Server.Controllers; 

[Route("configs")]
[ApiController]
[Authorize]
public sealed class ConfigController : Controller {

    private readonly IPlayerService _playerService;
    private readonly IConfigService _configService;

    public ConfigController(IPlayerService playerService, IConfigService configService) {
        _playerService = playerService;
        _configService = configService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> DownloadConfig(string id) {
        
        var player = await _playerService.GetFromHttpContext(HttpContext);
        if (player == null)
            return Unauthorized();

        var config = await _configService.DownloadConfig(player, id);
        if (config == null)
            return NoContent();

        return Content(config, "application/json", Encoding.UTF8);

    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult> UploadConfig(string id) {
        
        Request.EnableBuffering();
        Request.Body.Position = 0;

        var data = await new StreamReader(Request.Body).ReadToEndAsync();
        if (data.IsNullOrEmpty())
            return BadRequest();

        var player = await _playerService.GetFromHttpContext(HttpContext);
        if (player == null)
            return Unauthorized();

        var success = await _configService.UploadConfig(player, id, data);

        return success ? Ok() : this.InternalError();
        
    }

}