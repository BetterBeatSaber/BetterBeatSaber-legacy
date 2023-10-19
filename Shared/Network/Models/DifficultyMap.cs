using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Network.Enums;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Models; 

public struct DifficultyMap : INetSerializable {

    public MapType MapType { get; set; }
    
    public MapDifficulty Difficulty { get; set; }
    
    public float NotesPerSecond { get; set; }
    public float NoteJumpSpeed { get; set; }
    
    // If Custom Map
    public float? ScoreSaberStars { get; set; }
    public float? BeatLeaderStars { get; set; }

    public void Serialize(NetDataWriter writer) {
        
        writer.Put((byte) MapType);
        writer.Put((byte) Difficulty);
        writer.Put(NotesPerSecond);
        writer.Put(NoteJumpSpeed);

        if (MapType != MapType.Custom)
            return;
        
        writer.Put(ScoreSaberStars ?? -1);
        writer.Put(BeatLeaderStars ?? -1);

    }

    public void Deserialize(NetDataReader reader) {

        MapType = (MapType) reader.GetByte();
        Difficulty = (MapDifficulty) reader.GetByte();
        NotesPerSecond = reader.GetFloat();
        NoteJumpSpeed = reader.GetFloat();
        
        if (MapType != MapType.Custom)
            return;

        ScoreSaberStars = reader.GetFloat();
        BeatLeaderStars = reader.GetFloat();

    }

}