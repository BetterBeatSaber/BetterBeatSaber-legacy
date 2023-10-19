using BetterBeatSaber.Server.Interfaces;

using Octokit;

namespace BetterBeatSaber.Server.Services.Interfaces; 

public interface IGithubService : IInitializable {

    public string LatestVersion { get; }

    public Task<Release?> GetLatestRelease();
    public Task<Release> GetRelease(string version);

}