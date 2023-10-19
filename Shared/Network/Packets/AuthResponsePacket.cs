using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Network.Packets; 

public struct AuthResponsePacket : INetSerializable {

    public string ServerName { get; set; }
    public bool Success { get; set; }
    public string? Reason { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(ServerName);
        writer.Put(Success);
        writer.Put(Reason != null);
        if(Reason != null)
            writer.Put(Reason);
    }

    public void Deserialize(NetDataReader reader) {
        ServerName = reader.GetString();
        Success = reader.GetBool();
        if (reader.GetBool())
            Reason = reader.GetString();
    }

}