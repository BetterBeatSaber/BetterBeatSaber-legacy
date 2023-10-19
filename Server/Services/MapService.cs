using BeatSaverSharp;

using BetterBeatSaber.Server.Services.Interfaces;

namespace BetterBeatSaber.Server.Services; 

public sealed class MapService : IMapService {

    public byte[] MapCacheData { get; set; } = Array.Empty<byte>();
    
    private readonly BeatSaver _beatSaver;

    public MapService() {
        _beatSaver = new BeatSaver("Better Beat Saber", new Version("0.0.1"));
    }

}