using BetterBeatSaber.Server.Services.Interfaces;
using BetterBeatSaber.Shared.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterBeatSaber.Server.Controllers;

[Route("versions")]
[ApiController]
public sealed class VersionController : Controller {

    private readonly IGithubService _githubService;
    private readonly IPlayerService _playerService;
    private readonly IModuleService _moduleService;

    public VersionController(IGithubService githubService, IPlayerService playerService, IModuleService moduleService) {
        _githubService = githubService;
        _playerService = playerService;
        _moduleService = moduleService;
    }

    [HttpGet("latest")]
    public ActionResult GetLatestVersion() => Ok(_githubService.LatestVersion[1..]);

    [Authorize]
    [HttpGet("{version}/modules")]
    public async Task<ActionResult<List<ModuleManifest>>> GetModules([FromRoute] string version) {
        
        var player = await _playerService.GetFromHttpContext(HttpContext);
        if (player == null)
            return Unauthorized();

        var modules = await _moduleService.GetModulesByVersion(version);
        
        return modules.Where(module => player.Role >= module.RequiredRole).ToList();
        
    }
    
}