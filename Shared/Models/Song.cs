using System;

using BetterBeatSaber.Shared.Serialization;

using ISerializable = BetterBeatSaber.Shared.Serialization.ISerializable;

namespace BetterBeatSaber.Shared.Models; 

public struct Song : ISerializable {

    public string Key { get; set; }
    public string Hash { get; set; }
    public string SongName { get; set; }
    public string SongSubName { get; set; }
    public string SongAuthorName { get; set; }
    public string LevelAuthorName { get; set; }
    public DifficultySong[] Difficulties { get; set; }
    public SongCharacteristic Characteristics { get; set; }
    public DateTime Uploaded { get; set; }
    public string Uploader { get; set; }
    public float Bpm { get; set; }
    public int UpVotes { get; set; }
    public int DownVotes { get; set; }
    public int Duration { get; set; }

    public void Serialize(ByteBuffer buffer) {
        buffer.WriteString(Key);
        buffer.WriteString(Hash);
        buffer.WriteString(SongName);
        buffer.WriteString(SongSubName);
        buffer.WriteString(SongAuthorName);
        buffer.WriteString(LevelAuthorName);
        buffer.WriteArray(Difficulties);
        buffer.WriteEnum(Characteristics);
        buffer.WriteDateTime(Uploaded);
        buffer.WriteString(Uploader);
        buffer.WriteFloat32(Bpm);
        buffer.WriteInt32(UpVotes);
        buffer.WriteInt32(DownVotes);
        buffer.WriteInt32(Duration);
    }

    public void Deserialize(ByteBuffer buffer) {
        Key = buffer.ReadString();
        Hash = buffer.ReadString();
        SongName = buffer.ReadString();
        SongSubName = buffer.ReadString();
        SongAuthorName = buffer.ReadString();
        LevelAuthorName = buffer.ReadString();
        Difficulties = buffer.ReadArray<DifficultySong>();
        Characteristics = buffer.ReadEnum<SongCharacteristic>();
        Uploaded = buffer.ReadDateTime();
        Uploader = buffer.ReadString();
        Bpm = buffer.ReadFloat32();
        UpVotes = buffer.ReadInt32();
        DownVotes = buffer.ReadInt32();
        Duration = buffer.ReadInt32();
    }
    
    public struct DifficultySong : ISerializable {

        public SongDifficulty Difficulty { get; set; }
        public SongCharacteristic Characteristic { get; set; }
        public float Stars { get; set; }
        public float StarsBl { get; set; }
        public bool Ranked { get; set; }
        public bool RankedBl { get; set; }
        public DateTime RankedUpdateTime { get; set; }
        public int Bombs { get; set; }
        public int Notes { get; set; }
        public int Obstacles { get; set; }
        public float Njs { get; set; }
        public float NjsOffset { get; set; }
        public SongRequirement Requirements { get; set; }

        public void Serialize(ByteBuffer buffer) {
            buffer.WriteEnum(Difficulty);
            buffer.WriteEnum(Characteristic);
            buffer.WriteFloat32(Stars);
            buffer.WriteFloat32(StarsBl);
            buffer.WriteBool(Ranked);
            buffer.WriteBool(RankedBl);
            buffer.WriteDateTime(RankedUpdateTime);
            buffer.WriteInt32(Bombs);
            buffer.WriteInt32(Notes);
            buffer.WriteInt32(Obstacles);
            buffer.WriteFloat32(Njs);
            buffer.WriteFloat32(NjsOffset);
            buffer.WriteEnum(Requirements);
        }

        public void Deserialize(ByteBuffer buffer) {
            Difficulty = buffer.ReadEnum<SongDifficulty>();
            Characteristic = buffer.ReadEnum<SongCharacteristic>();
            Stars = buffer.ReadFloat32();
            StarsBl = buffer.ReadFloat32();
            Ranked = buffer.ReadBool();
            RankedBl = buffer.ReadBool();
            RankedUpdateTime = buffer.ReadDateTime();
            Bombs = buffer.ReadInt32();
            Notes = buffer.ReadInt32();
            Obstacles = buffer.ReadInt32();
            Njs = buffer.ReadFloat32();
            NjsOffset = buffer.ReadFloat32();
            Requirements = buffer.ReadEnum<SongRequirement>();
        }

    }

    public enum SongDifficulty : byte {

        Easy = 0,
        Normal = 1,
        Hard = 2,
        Expert = 3,
        ExpertPlus = 4

    }

    [Flags]
    public enum SongCharacteristic : byte {

        Standard = 0,
        OneSaber = 1,
        NoArrows = 2,
        Degree90 = 3,
        Degree360 = 4,
        Lightshow = 5,
        Lawless = 6,
        Legacy = 7

    }

    [Flags]
    public enum SongRequirement : byte {

        None = 0,
        Chroma = 1,
        Cinema = 2,
        NoodleExtensions = 3,
        MappingExtensions = 4

    }

}