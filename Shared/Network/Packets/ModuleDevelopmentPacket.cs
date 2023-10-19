#if DEBUG
using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Network.Packets;

public struct ModuleDevelopmentPacket : INetSerializable {

    public string ModuleId { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(ModuleId);
    }

    public void Deserialize(NetDataReader reader) {
        ModuleId = reader.GetString();
    }

}
#endif
