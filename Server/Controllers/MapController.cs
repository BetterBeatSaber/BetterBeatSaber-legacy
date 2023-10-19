using BetterBeatSaber.Server.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace BetterBeatSaber.Server.Controllers; 

[Route("maps")]
[ApiController]
public sealed class MapController : Controller {

    private readonly IMapService _mapService;
    private readonly HttpClient _httpClient;

    public MapController(HttpClient httpClient, IMapService mapService) {
        _httpClient = httpClient;
        _mapService = mapService;
    }

    [HttpGet("download/cache")]
    public ActionResult DownloadCache() =>
        File(new MemoryStream(_mapService.MapCacheData), "application/octet-stream");
    
    [HttpGet("download/{hash}")]
    public async Task<ActionResult> Download(string hash) {

        var stream = await _httpClient.GetStreamAsync($"https://r2cdn.beatsaver.com/{hash}.zip");

        return new FileStreamResult(stream, "");
        
    }

}