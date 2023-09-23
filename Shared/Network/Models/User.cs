using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Models; 

public struct User : INetSerializable {

    public string PlatformId { get; set; }
    public string Name { get; set; }
    public string AvatarUrl { get; set; }

    public void Serialize(NetDataWriter writer) {
        writer.Put(PlatformId);
        writer.Put(Name);
        writer.Put(AvatarUrl);
    }

    public void Deserialize(NetDataReader reader) {
        PlatformId = reader.GetString();
        Name = reader.GetString();
        AvatarUrl = reader.GetString();
    }

}