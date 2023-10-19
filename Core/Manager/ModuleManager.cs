using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using BetterBeatSaber.Core.Api;
using BetterBeatSaber.Core.Config;
using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Game;
using BetterBeatSaber.Core.Network;
using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Network.Packets;

using Newtonsoft.Json;

namespace BetterBeatSaber.Core.Manager; 

public sealed class ModuleManager : Manager<ModuleManager> {

    #if !DEBUG
    private static string DownloadUrl => $"https://github.com/BetterBeatSaber/BetterBeatSaber/releases/download/v{BetterBeatSaber.Version}";
    #else
    private static string DownloadUrl => "http://localhost:5295/data";
    #endif
    
    public static string ModulesDirectory => Path.Combine(BeatSaber.GameDirectory, "BetterBeatSaber", "Modules");
    
    public List<Module> Modules { get; private set; } = new();

    public LoadState State { get; private set; } = LoadState.None;

    #region Events

    public event Action<ModuleManifest>? OnModuleManifestLoaded;
    
    public event Action<Module>? OnModuleLoaded;
    
    public event Action<Module>? OnModuleInitialized;
    public event Action<Module>? OnModuleExited;

    public event Action<Module>? OnModuleEnabled;
    public event Action<Module>? OnModuleDisabled;
    
    #endregion

    private readonly Dictionary<string, Module> _moduleCache = new();
    private readonly Dictionary<string, ModuleManifest> _moduleManifestCache = new();
    
    #region Init & Exit

    public override void Init() {

        foreach (var module in CoreConfig.Instance.Modules)
            AsyncHelper.RunSync(async () => await Install(module));

        if (!Directory.Exists(ModulesDirectory))
            Directory.CreateDirectory(ModulesDirectory);
        
        foreach (var file in Directory.GetFiles(ModulesDirectory).Where(file => file.EndsWith(".dll")))
            Load(file);
        
        foreach (var module in Modules)
            InitModule(module);

        #if DEBUG
        NetworkClient.Instance.RegisterPacketHandler<ModuleDevelopmentPacket>(OnModuleDevelopmentPacketReceived);
        #endif
        
        State = LoadState.Initialized;

    }

    public override void Exit() {
        
        #if DEBUG
        NetworkClient.Instance.UnregisterPacketHandler<ModuleDevelopmentPacket>();
        #endif
        
        foreach (var module in Modules)
            ExitModule(module);
        
        State = LoadState.Exited;
        
    }

    #endregion

    #region Enable & Disable

    public override void Enable() {
        
        foreach (var module in Modules)
            EnableModule(module);
        
        State = LoadState.Enabled;
        
    }

    public override void Disable() {
        
        foreach (var module in Modules)
            DisableModule(module);
        
        State = LoadState.Disabled;
        
    }

    #endregion

    #region Module Actions

    private void InitModule(Module module) {
        try {
            module.Init();
            OnModuleInitialized?.Invoke(module);
        } catch (Exception exception) {
            Logger.Warn($"Failed to initialize module {module.Id}");
            Logger.Warn(exception);
        }
    }
    
    private void ExitModule(Module module) {
        try {
            module.Exit();
            OnModuleExited?.Invoke(module);
        } catch (Exception exception) {
            Logger.Warn($"Failed to exit module {module.Id}");
            Logger.Warn(exception);
        }
    }

    private void EnableModule(Module module) {
        try {
            module.Enable();
            OnModuleEnabled?.Invoke(module);
        } catch (Exception exception) {
            Logger.Warn($"Failed to enable module {module.Id}");
            Logger.Warn(exception);
        }
    }

    private void DisableModule(Module module) {
        try {
            module.Disable();
            OnModuleDisabled?.Invoke(module);
        } catch (Exception exception) {
            Logger.Warn($"Failed to disable module {module.Id}");
            Logger.Warn(exception);
        }
    }
    
    #endregion

    #region Methods

    #region Public

    public bool IsInstalled(string id) =>
        Modules.Any(module => module.Id == id);
    
    public Module? GetModule(string id) =>
        Modules.FirstOrDefault(module => module.Id == id);
    
    public Module? GetModule(Assembly assembly) =>
        Modules.FirstOrDefault(module => module.GetType().Assembly == assembly);

    #endregion

    #region Private

    internal async Task<Module?> Install(string id) {

        #if !DEBUG
        if (_moduleCache.TryGetValue(id, out var module)) {
            Modules.Add(module);
            RunModule(module);
            goto addToConfig;
        }
        #endif

        var manifest = await LoadManifest(id);
        if (manifest == null)
            return null;
        
        var rawAssembly = await ApiClient.Instance.HttpClient.GetByteArrayAsync($"{DownloadUrl}/{id}.dll");
        if (rawAssembly == null)
            return null;

        var assembly = Assembly.Load(rawAssembly);

        #if DEBUG
        var
        #endif
            module = ConstructAndRunModule(manifest, assembly, false);
        
        #region Add to Config

        #if !DEBUG
        addToConfig:
        #endif

        if (CoreConfig.Instance.Modules.Contains(id))
            return module;

        CoreConfig.Instance.Modules.Add(id);
        ConfigManager.Instance.SaveConfig(CoreConfig.Instance);

        #endregion

        return module;

    }

    internal bool Uninstall(string id) {
        
        if (State == LoadState.None)
            return _moduleManifestCache.Remove(id);
        
        var module = Modules.FirstOrDefault(m => m.Id == id);
        if (module == null)
            return false;

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (State) {
            case LoadState.Initialized:
                ExitModule(module);
                break;
            case LoadState.Enabled:
                DisableModule(module);
                ExitModule(module);
                break;
        }

        Modules.Remove(module);

        if (!CoreConfig.Instance.Modules.Contains(id))
            return true;

        CoreConfig.Instance.Modules.Remove(id);
        ConfigManager.Instance.SaveConfig(CoreConfig.Instance);

        return true;
        
    }
    
    public Module? Load(string path) {

        var assembly = Assembly.LoadFrom(path);

        var manifest = LoadManifestFromAssembly(assembly);
        if (manifest == null)
            return null;

        if (!_moduleCache.TryGetValue(manifest.Id, out var module))
            return ConstructAndRunModule(manifest, assembly, true);

        Modules.Add(module);
        
        RunModule(module);
        
        return module;

    }

    private async Task<ModuleManifest?> LoadManifest(string id) {
        
        if (_moduleManifestCache.TryGetValue(id, out var moduleManifest))
            return moduleManifest;

        var raw = await ApiClient.Instance.HttpClient.GetStringAsync($"{DownloadUrl}/{id}.json");
        var manifest = LoadManifestFromString(raw);
        
        if(manifest != null)
            _moduleManifestCache.Add(manifest.Id, manifest);
        
        return manifest;

    }

    private ModuleManifest? LoadManifestFromAssembly(Assembly assembly) {

        if (_moduleManifestCache.TryGetValue(assembly.FullName, out var moduleManifest))
            return moduleManifest;
        
        var manifestStream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().FirstOrDefault(resource => resource.EndsWith("module.json")));
        if (manifestStream == null)
            return null;

        using var streamReader = new StreamReader(manifestStream);
        moduleManifest = LoadManifestFromString(streamReader.ReadToEnd());
        streamReader.Close();

        if (moduleManifest == null)
            return moduleManifest;

        _moduleManifestCache.Add(assembly.FullName, moduleManifest);
        
        return moduleManifest;

    }
    
    private ModuleManifest? LoadManifestFromString(string? raw) {

        if (raw == null)
            return null;
        
        var manifest = JsonConvert.DeserializeObject<ModuleManifest?>(raw, HttpContentExtensions.DefaultJsonSerializerSettings);
        
        if(manifest != null)
            OnModuleManifestLoaded?.Invoke(manifest);

        return manifest;

    }
    
    private Module? ConstructAndRunModule(ModuleManifest? moduleManifest, Assembly? assembly, bool isLocal) {

        if (assembly == null || moduleManifest == null)
            return null;
        
        var mainType = assembly.GetTypes().FirstOrDefault(type => type.IsSubclassOf(typeof(Module)));

        var module = mainType?.Construct<Module>(new Dictionary<string, object> {
            { nameof(Module.Manifest), moduleManifest },
            { nameof(Module.Logger), BetterBeatSaber.Logger.GetChildLogger(moduleManifest.Name) },
            { nameof(Module.IsLocal), isLocal }
        });

        if (module == null)
            return null;

        _moduleCache.Add(moduleManifest.Id, module);
        
        Modules.Add(module);
        
        OnModuleLoaded?.Invoke(module);

        RunModule(module);
        
        return module;

    }

    private void RunModule(Module module) {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (State) {
            case LoadState.Initialized:
                InitModule(module);
                break;
            case LoadState.Enabled:
                InitModule(module);
                EnableModule(module);
                break;
        }
    }

    #endregion

    #endregion
    
    #if DEBUG
    private void OnModuleDevelopmentPacketReceived(ModuleDevelopmentPacket packet) {
        Uninstall(packet.ModuleId);
        AsyncHelper.RunSync(async () => await Install(packet.ModuleId));
    }
    #endif
    
    public static async Task<IEnumerable<ModuleManifest>> FetchAvailableModules() =>
        await ApiClient.Instance.Get<List<ModuleManifest>>($"/versions/{BetterBeatSaber.Version}/modules") ?? Enumerable.Empty<ModuleManifest>();
    
    public enum LoadState {

        None,
        Initialized,
        Enabled,
        Disabled,
        Exited

    }
    
}