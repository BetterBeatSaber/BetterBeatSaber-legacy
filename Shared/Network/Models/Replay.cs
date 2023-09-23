using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Models; 

public struct Replay : INetSerializable {

    public Map Map { get; set; }
    public User User { get; set; }

    public void Serialize(NetDataWriter writer) {
        writer.Put(Map);
        writer.Put(User);
    }

    public void Deserialize(NetDataReader reader) {
        Map = reader.Get<Map>();
        User = reader.Get<User>();
    }

}