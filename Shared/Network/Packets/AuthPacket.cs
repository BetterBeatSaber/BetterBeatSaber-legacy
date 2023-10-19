using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Network.Packets;

public struct AuthPacket : INetSerializable {

    public string Session { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(Session);
    }

    public void Deserialize(NetDataReader reader) {
        Session = reader.GetString();
    }

}