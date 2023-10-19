using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using BetterBeatSaber.Core.Api;
using BetterBeatSaber.Core.Config.Converters;
using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Game;
using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Core.Utilities;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BetterBeatSaber.Core.Config;

public sealed class ConfigManager : Manager<ConfigManager> {

    public static string ConfigsDirectory => Path.Combine(BeatSaber.GameDirectory, "BetterBeatSaber", "Configs");
    
    internal static readonly JsonSerializerSettings JsonSerializerSettings = new() {
        Formatting = Formatting.Indented,
        ContractResolver = new DefaultContractResolver {
            NamingStrategy = new SnakeCaseNamingStrategy()
        },
        Converters = new List<JsonConverter> {
            new ColorConverter(),
            new Vector2Converter(),
            new Vector3Converter(),
            new QuaternionConverter()
        }
    };

    #region Init & Exit
    
    public override void Init() {
        
        if (!Directory.Exists(ConfigsDirectory))
            Directory.CreateDirectory(ConfigsDirectory);
        
        AuthManager.Instance.OnAuthenticated += OnAuthenticated;
        
    }

    public override void Exit() {
        AuthManager.Instance.OnAuthenticated -= OnAuthenticated;
    }

    #endregion

    #region Event Handlers

    private void OnAuthenticated() {

        while (!_uploadQueue.IsEmpty) {
            if(_uploadQueue.TryDequeue(out var config))
                ThreadDispatcher.Enqueue(SaveConfigToCloud(config));
        }
        
        LoadConfigsFromCloud();
        
    }

    #endregion

    private readonly List<Config> _configs = new();

    private readonly ConcurrentQueue<Config> _uploadQueue = new();
    
    #region Methods

    #region Public

    public T? CreateConfig<T>(string id, bool saveLocal = false) where T : Config<T> {
        
        var config = typeof(T).Construct<T>(new Dictionary<string, object> {
            { nameof(Config.Id), id },
            { nameof(Config.SaveLocal), saveLocal }
        });
        
        if (config == null)
            return null;

        if (_configs.Any(c => c.Id == id))
            _configs.RemoveAll(c => c.Id == id);
        
        LoadConfig(config);
        
        _configs.Add(config);

        return (T?) config;
        
    }

    public T? CreateConfig<T>(Module module) where T : Config<T> => CreateConfig<T>(module.Id, module.IsLocal);

    #endregion

    #region Private
    
    internal void LoadConfig(Config config) {
        try {
            
            var path = Path.Combine(ConfigsDirectory, $"{config.Id}.json");
            if (!File.Exists(path)) {
                SaveConfig(config);
                return;
            }

            var raw = File.ReadAllText(path);
            if (raw.Length < 2) {
                SaveConfig(config);
                return;
            }

            PopulateConfig(config, raw, false);
            
        } catch (Exception exception) {
            Logger.Error("Failed to load Config for Module: {0}", config.Id);
            Logger.Error(exception);
        }
    }

    internal void SaveConfig(Config config) {
        try {
            
            File.WriteAllText(Path.Combine(ConfigsDirectory, $"{config.Id}.json"), JsonConvert.SerializeObject(config, JsonSerializerSettings));

            if (AuthManager.Instance.IsAuthenticated) {
                ThreadDispatcher.Enqueue(SaveConfigToCloud(config));
            } else {
                _uploadQueue.Enqueue(config);
            }
            
        } catch (Exception exception) {
            Logger.Error($"Failed to save Config for Module: {config.Id}");
            Logger.Error(exception);
        }
    }

    private void LoadConfigsFromCloud() {

        if (!CoreConfig.Instance.ConfigCloudSynchronization)
            return;
        
        foreach (var config in _configs)
            ThreadDispatcher.Enqueue(LoadConfigFromCloud(config));
        
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    private IEnumerator LoadConfigFromCloud(Config config) {

        var request = new ApiRequest<string>($"/configs/{config.Id}") {
            ResponseRaw = true
        };

        yield return request.Send();

        if (request.Failed || request.Response == null || request.Response == null)
            yield break;
            
        PopulateConfig(config, request.Response, true);
        
    }

    private IEnumerator SaveConfigToCloud(Config config) {
        
        var request = new ApiRequest<string>($"/configs/{config.Id}", "PUT") {
            BodyRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(config, JsonSerializerSettings)),
            ContentType = "application/json; charset=utf-8"
        };

        yield return request.Send();
        
        if(!request.Failed)
            Logger.Info("Uploaded config: {0}", config.Id);
        else
            Logger.Warn("Failed to upload config: {0}", config.Id);
        
    }

    private void PopulateConfig(Config config, string raw, bool fromCloud) {
        try {
            JsonConvert.PopulateObject(raw, config, JsonSerializerSettings);
            if (config is IConfigLoadedHandler handler)
                handler.OnLoaded(fromCloud);
        } catch (Exception exception) {
            Logger.Error("Failed to load Config for Module: {0} ({1})", config.Id, fromCloud ? "Cloud" : "Local");
            Logger.Error(exception);
        }
    }
    
    #endregion
    
    #endregion
    
}