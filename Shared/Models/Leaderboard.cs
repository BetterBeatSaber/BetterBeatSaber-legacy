using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Models; 

public struct Leaderboard : INetSerializable {

    public string Country { get; set; }
    public double Pp { get; set; }
    public uint GlobalRank { get; set; }
    public uint LocalRank { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(Country);
        writer.Put(Pp);
        writer.Put(GlobalRank);
        writer.Put(LocalRank);
    }

    public void Deserialize(NetDataReader reader) {
        Country = reader.GetString();
        Pp = reader.GetDouble();
        GlobalRank = reader.GetUInt();
        LocalRank = reader.GetUInt();
    }

}