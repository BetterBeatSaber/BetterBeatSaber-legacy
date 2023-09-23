using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using IPA;
using IPA.Logging;

using JetBrains.Annotations;

namespace BetterBeatSaber.Loader; 

// ReSharper disable UnusedMember.Global

[Plugin(RuntimeOptions.SingleStartInit)]
public sealed class BetterBeatSaberLoader {
    
    [UsedImplicitly]
    private static Logger _logger = null!;

    private object? _core;

    [Init]
    public BetterBeatSaberLoader(Logger logger) {

        _logger = logger;
        
        RunInit();
        
        InvokeCoreMethod("Init");
        
    }
    
    [OnStart]
    public void OnStart() => InvokeCoreMethod("Start");
    
    [OnEnable]
    public void OnEnable() => InvokeCoreMethod("Enable");

    [OnDisable]
    public void OnDisable() => InvokeCoreMethod("Disable");
    
    [OnExit]
    public void OnExit() => InvokeCoreMethod("Exit");
    
    private void InvokeCoreMethod(string methodName) {
        try {
            _core?.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance)!.Invoke(_core, null);
        } catch (Exception exception) {
            _logger.Error($"Failed to run Better Beat Saber ({methodName})");
            _logger.Error(exception);
        }
    }

    private byte _failCount = 0;
    
    private void RunInit() {
        try {
            AsyncHelper.RunSync(Init);
        } catch (Exception exception) {
            _failCount++;
            _logger.Error("Failed to run Better Beat Saber (Init)");
            _logger.Error(exception);
            if(_failCount < 3)
                RunInit();
        }
    }

    private async Task Init() {

        #if DEBUG
        
        var rawAssembly = await new HttpClient().GetByteArrayAsync("http://localhost:5295/data/core.dll");
        
        #else
        
        var httpClient = new HttpClient();
        
        var latestVersion = await httpClient.GetStringAsync("https://api.betterbs.xyz/versions/latest");
        var rawAssembly = await httpClient.GetByteArrayAsync($"https://github.com/BetterBeatSaber/BetterBeatSaber/releases/download/{latestVersion}/BetterBeatSaber.Core.dll");
        
        #endif
        
        if (rawAssembly == null || rawAssembly.Length == 0)
            throw new Exception("Failed to download latest assembly");

        var coreAssembly = Assembly.Load(rawAssembly);
        if (coreAssembly == null)
            throw new Exception("Failed to load assembly");

        _core = coreAssembly
                .GetType("BetterBeatSaber.Core.BetterBeatSaber")!
                .GetConstructor(new[] { typeof(Logger) })!
                .Invoke(new object[] { _logger });
        
    }

}