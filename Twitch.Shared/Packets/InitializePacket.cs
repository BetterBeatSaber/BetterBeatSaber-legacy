using BetterBeatSaber.Twitch.Shared.Enums;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Twitch.Shared.Packets; 

public struct InitializePacket : INetSerializable {

    public FeatureFlag Features { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put((ushort) Features);
    }

    public void Deserialize(NetDataReader reader) {
        Features = (FeatureFlag) reader.GetUShort();
    }

}