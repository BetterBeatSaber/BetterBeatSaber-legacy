using BetterBeatSaber.Server.Services.Interfaces;

using JetBrains.Annotations;

using Octokit;

namespace BetterBeatSaber.Server.Services; 

public sealed class GithubService : IGithubService {

    private readonly IGitHubClient _gitHubClient;
    
    [UsedImplicitly]
    public string LatestVersion { get; private set; } = null!;

    public GithubService(IGitHubClient gitHubClient) {
        _gitHubClient = gitHubClient;
    }

    public Task Init() => GetLatestRelease();
    
    public async Task<Release?> GetLatestRelease() {
        try {
            
            var release = await _gitHubClient.Repository.Release.GetLatest("BetterBeatSaber", "BetterBeatSaber");

            if(release != null)
                LatestVersion = release.Name;

            return release;
            
        } catch (Exception _) {
            return null;
        }
    }

    public async Task<Release> GetRelease(string version) {
        return await _gitHubClient.Repository.Release.Get("BetterBeatSaber", "BetterBeatSaber", version);
    }

}