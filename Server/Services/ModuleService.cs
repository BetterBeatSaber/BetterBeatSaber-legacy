using BetterBeatSaber.Server.Extensions;
using BetterBeatSaber.Server.Interfaces;
using BetterBeatSaber.Server.Services.Interfaces;
using BetterBeatSaber.Shared.Models;

using Octokit;

namespace BetterBeatSaber.Server.Services; 

public sealed class ModuleService : IModuleService {

    public List<ModuleManifest> Modules { get; private set; } = new();

    private readonly IGithubService _githubService;
    private readonly HttpClient _httpClient;

    public ModuleService(IGithubService githubService, HttpClient httpClient) {
        _githubService = githubService;
        _httpClient = httpClient;
    }

    public async Task Init() {
        Modules = await ParseRelease(await _githubService.GetLatestRelease());
    }

    public async Task<List<ModuleManifest>> GetModulesByVersion(string version) {
        if (version == _githubService.LatestVersion)
            return Modules;
        return await GetModules(version);
    }

    private async Task<List<ModuleManifest>> GetModules(string version) {
        return await ParseRelease(await _githubService.GetRelease(version));
    }

    private async Task<List<ModuleManifest>> ParseRelease(Release release) {
        var modules = new List<ModuleManifest>();
        foreach (var asset in release.Assets.Where(asset => asset.Name.EndsWith(".json"))) {
            var module = await _httpClient.GetJsonAsync<ModuleManifest>(asset.BrowserDownloadUrl);
            if(module != null)
                modules.Add(module);
        }
        return modules;
    }

}