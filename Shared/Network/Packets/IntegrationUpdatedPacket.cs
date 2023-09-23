using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Network.Packets; 

public struct IntegrationUpdatedPacket : INetSerializable {

    public IntegrationType Type { get; set; }
    public Integration Integration { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put((byte) Type);
        writer.Put(Integration);
    }

    public void Deserialize(NetDataReader reader) {
        Type = (IntegrationType) reader.GetByte();
        Integration = reader.Get<Integration>();
    }

}