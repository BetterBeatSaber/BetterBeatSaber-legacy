using BetterBeatSaber.Server.Models;

namespace BetterBeatSaber.Server.Services.Interfaces; 

public interface IConfigService {

    public Task<string?> DownloadConfig(Player player, string id);
    public Task<bool> UploadConfig(Player player, string id, string config);
    public Task<bool> DeleteAllConfigs(Player player);

}