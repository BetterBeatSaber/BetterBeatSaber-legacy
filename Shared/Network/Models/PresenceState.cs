using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Network.Enums;
using BetterBeatSaber.Shared.Network.Interfaces;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Models; 

public struct PlayingMapPresenceState : IPresenceState {

    public Status Status => Status.PlayingMap;
    
    public bool Paused { get; set; }
    public Rank Rank { get; set; }
    public float Score { get; set; }
    public uint Misses { get; set; }

    public void Serialize(NetDataWriter writer) {
        writer.Put(Paused);
        writer.Put((byte) Rank);
        writer.Put(Score);
        writer.Put(Misses);
    }

    public void Deserialize(NetDataReader reader) {
        Paused = reader.GetBool();
        Rank = (Rank) reader.GetByte();
        Score = reader.GetFloat();
        Misses = reader.GetUInt();
    }

}