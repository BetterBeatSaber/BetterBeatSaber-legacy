using BetterBeatSaber.Shared.Enums;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Models;

public struct Map : INetSerializable {

    public MapType Type { get; set; }
    
    public string SongName { get; set; }
    public string SongAuthor { get; set; }
    public string LevelAuthor { get; set; }
    
    // If Custom Map
    public string? Hash { get; set; }

    public void Serialize(NetDataWriter writer) {
        
        writer.Put((byte) Type);
        writer.Put(SongName);
        writer.Put(SongAuthor);
        writer.Put(LevelAuthor);
        
        if(Type == MapType.Custom)
            writer.Put(Hash);
        
    }

    public void Deserialize(NetDataReader reader) {
        
        Type = (MapType) reader.GetByte();
        SongName = reader.GetString();
        SongAuthor = reader.GetString();
        LevelAuthor = reader.GetString();

        if (Type == MapType.Custom)
            Hash = reader.GetString();

    }

}