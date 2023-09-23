using BetterBeatSaber.Server.Services.Interfaces;
using BetterBeatSaber.Shared.Models;

using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

namespace BetterBeatSaber.Server.Services; 

public sealed class ModuleDevelopmentService : IModuleService {

    private List<ModuleManifest> Modules { get; set; } = new();

    public Task<List<ModuleManifest>> GetModulesByVersion(string version) => Task.FromResult(Modules);

    public async Task Init() {
        foreach (var path in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Data")).Where(p => p.EndsWith(".json"))) {
            var module = JsonConvert.DeserializeObject<ModuleManifest>(await File.ReadAllTextAsync(path));
            if(module != null && !module.Id.IsNullOrEmpty() && !module.Name.IsNullOrEmpty())
                Modules.Add(module);
        }
    }

}