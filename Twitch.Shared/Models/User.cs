using LiteNetLib.Utils;

namespace BetterBeatSaber.Twitch.Shared.Models; 

public struct User : INetSerializable {

    public string Id { get; set; }
    public string Name { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(Id);
        writer.Put(Name);
    }

    public void Deserialize(NetDataReader reader) {
        Id = reader.GetString();
        Name = reader.GetString();
    }

}