using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using BetterBeatSaber.Core.Api;
using BetterBeatSaber.Core.Config.Converters;
using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Game;
using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Core.Threading;

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
    }
    
    #endregion

    private readonly List<Config> _configs = new();

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

    internal void LoadConfig(Config config) {
        try {
            
            var data = !config.SaveLocal ? DownloadConfig(config.Id) : (File.Exists(Path.Combine(ConfigsDirectory, $"{config.Id}.json")) ? File.ReadAllText(Path.Combine(ConfigsDirectory, $"{config.Id}.json")) : string.Empty);
            
            if (data.Length >= 2)
                JsonConvert.PopulateObject(data, config, JsonSerializerSettings);
            else
                SaveConfig(config);
            
        } catch (Exception exception) {
            Logger.Error($"Failed to load Config for Module: {config.Id}");
            Logger.Error(exception);
        }
    }

    internal void SaveConfig(Config config) {
        try {
            
            if (!config.SaveLocal) {
                UploadConfig(config.Id, JsonConvert.SerializeObject(config, JsonSerializerSettings));
            } else {
                File.WriteAllText(Path.Combine(ConfigsDirectory, $"{config.Id}.json"), JsonConvert.SerializeObject(config, JsonSerializerSettings));
            }
            
        } catch (Exception exception) {
            Logger.Error($"Failed to save Config for Module: {config.Id}");
            Logger.Error(exception);
        }
    }
    
    internal string DownloadConfig(string id) =>
        AsyncHelper.RunSync(async () => await DownloadConfigAsync(id));
    
    internal async Task<string> DownloadConfigAsync(string id) {
        
        var response = await ApiClient.Instance.GetRaw($"/configs/{id}");
        if (response is not { IsSuccessStatusCode: true })
            return string.Empty;
        
        return await response.Content.ReadAsStringAsync();
        
    }

    internal void UploadConfig(string id, string config) {
        // ReSharper disable once AsyncVoidLambda
        ThreadDispatcher.EnqueueOffMain(async () => await UploadConfigAsync(id, config));
    }

    internal async Task<bool> UploadConfigAsync(string id, string config) {
        
        var response = await ApiClient.Instance.PutRaw($"/configs/{id}", new StringContent(config, Encoding.UTF8, "application/json"));
        if (response is not { IsSuccessStatusCode: true }) {
            Logger.Info($"Failed to upload config: \"{id}\"");
            return false;
        }
        
        Logger.Info($"Uploaded config: \"{id}\"");
        
        return true;
        
    }

    internal bool UploadConfigSync(string id, string config) =>
        AsyncHelper.RunSync(async () => await UploadConfigAsync(id, config));

}