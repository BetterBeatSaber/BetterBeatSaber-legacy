using BetterBeatSaber.Server.Network.Interfaces;
using BetterBeatSaber.Server.Services.Interfaces;
using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Network.Packets;

using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

namespace BetterBeatSaber.Server.Services; 

public sealed class ModuleDevelopmentService : IModuleService {

    public static string DataDirectory => Path.Combine(Environment.CurrentDirectory, "Data");
    
    private List<ModuleManifest> Modules { get; set; } = new();

    public Task<List<ModuleManifest>> GetModulesByVersion(string version) => Task.FromResult(Modules);

    private readonly ILogger<ModuleDevelopmentService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly FileSystemWatcher _fileSystemWatcher;
    
    public ModuleDevelopmentService(ILogger<ModuleDevelopmentService> logger, IServiceScopeFactory serviceScopeFactory) {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _fileSystemWatcher = new FileSystemWatcher {
            Path = DataDirectory,
            NotifyFilter = NotifyFilters.LastWrite,
            Filter = "*.dll",
            EnableRaisingEvents = true
        };
    }

    public async Task Init() {
        
        foreach (var path in Directory.GetFiles(DataDirectory).Where(p => p.EndsWith(".json"))) {
            var module = JsonConvert.DeserializeObject<ModuleManifest>(await File.ReadAllTextAsync(path));
            if(module != null && !module.Id.IsNullOrEmpty() && !module.Name.IsNullOrEmpty())
                Modules.Add(module);
        }
        
        _logger.LogInformation("Loaded {ModuleCount} modules", Modules.Count);
        
        _fileSystemWatcher.Changed += OnModuleChanged;
        
    }

    private void OnModuleChanged(object _, FileSystemEventArgs args) {

        if (args.Name == null)
            return;
        
        using var scope = _serviceScopeFactory.CreateScope();

        var server = scope.ServiceProvider.GetService<IServer>();
        if (server == null)
            return;

        var moduleId = args.Name[..^4];
        if (moduleId is null or "core")
            return;
        
        foreach (var connection in server.Connections) {
            connection.SendPacket(new ModuleDevelopmentPacket {
                ModuleId = moduleId
            });
        }
        
        _logger.LogInformation("Sent module update packet for {ModuleId}", moduleId);

    }

}