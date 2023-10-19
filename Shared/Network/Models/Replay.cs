using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Models; 

public struct Replay : INetSerializable {

    public Map Map { get; set; }
    public DifficultyMap Difficulty { get; set; }
    public User User { get; set; }

    public void Serialize(NetDataWriter writer) {
        writer.Put(Map);
        writer.Put(Difficulty);
        writer.Put(User);
    }

    public void Deserialize(NetDataReader reader) {
        Map = reader.Get<Map>();
        Difficulty = reader.Get<DifficultyMap>();
        User = reader.Get<User>();
    }

}