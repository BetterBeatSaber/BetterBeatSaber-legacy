using System;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace BetterBeatSaber.Core.Config;

public abstract class Config {

    [JsonIgnore]
    [UsedImplicitly]
    internal readonly string Id = null!;
    
    [JsonIgnore]
    [UsedImplicitly]
    internal readonly bool SaveLocal = false;
    
    [JsonIgnore]
    public bool Updated { get; set; }

    [Obsolete]
    public void Save() => ConfigManager.Instance.SaveConfig(this);

}

public abstract class Config<T> : Config where T : Config<T> {

    [JsonIgnore]
    public static T Instance { get; private set; } = null!;

    protected Config() {
        Instance = (T) this;
    }

}