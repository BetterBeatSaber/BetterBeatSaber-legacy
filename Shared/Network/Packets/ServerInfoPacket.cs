using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Network.Packets; 

public struct ServerInfoPacket : INetSerializable {

    public string ServerName { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(ServerName);
    }

    public void Deserialize(NetDataReader reader) {
        ServerName = reader.GetString();
    }

}