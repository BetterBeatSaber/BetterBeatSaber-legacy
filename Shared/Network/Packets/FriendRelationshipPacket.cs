using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Network.Packets; 

public struct FriendRelationshipPacket : INetSerializable {

    /// <summary>
    /// The (new) relationship between me and the other player
    /// </summary>
    public FriendRelationship Relationship { get; set; }
    
    /// <summary>
    /// The other player
    /// </summary>
    public Player Player { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put((byte) Relationship);
        writer.Put(Player);
    }

    public void Deserialize(NetDataReader reader) {
        Relationship = (FriendRelationship) reader.GetByte();
        Player = reader.Get<Player>();
    }

}