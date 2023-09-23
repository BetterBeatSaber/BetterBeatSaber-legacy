using BetterBeatSaber.Server.Interfaces;
using BetterBeatSaber.Shared.Models;

namespace BetterBeatSaber.Server.Services.Interfaces; 

public interface IModuleService : IInitializable {

    public Task<List<ModuleManifest>> GetModulesByVersion(string version);

}