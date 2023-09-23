using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Network.Packets;

public struct AuthPacket : INetSerializable {

    public string Session { get; set; }
    public string Version { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(Session);
        writer.Put(Version);
    }

    public void Deserialize(NetDataReader reader) {
        Session = reader.GetString();
        Version = reader.GetString();
    }

}