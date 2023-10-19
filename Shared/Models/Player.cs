using System;

using BetterBeatSaber.Shared.Enums;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Models; 

#pragma warning disable CS8618

public struct Player : INetSerializable, IEquatable<Player> {

    public ulong Id { get; set; }
    public string Name { get; set; }
    public string AvatarUrl { get; set; }
    public PlayerRole Role { get; set; }
    public PlayerFlag Flags { get; set; }
    public Leaderboard? ScoreSaber { get; set; }
    public Leaderboard? BeatLeader { get; set; }

    public void Serialize(NetDataWriter writer) {
        
        writer.Put(Id);
        writer.Put(Name);
        writer.Put(AvatarUrl);
        writer.Put((byte) Role);
        writer.Put((ushort) Flags);
        
        if (ScoreSaber != null) {
            writer.Put(true);
            writer.Put(ScoreSaber.Value);
        }

        // ReSharper disable once InvertIf
        if (BeatLeader != null) {
            writer.Put(true);
            writer.Put(BeatLeader.Value);
        }
        
    }

    public void Deserialize(NetDataReader reader) {
        
        Id = reader.GetULong();
        Name = reader.GetString();
        AvatarUrl = reader.GetString();
        Role = (PlayerRole) reader.GetByte();
        Flags = (PlayerFlag) reader.GetUShort();

        if (reader.GetBool())
            ScoreSaber = reader.Get<Leaderboard>();
        
        if (reader.GetBool())
            BeatLeader = reader.Get<Leaderboard>();

    }

    public bool Equals(Player other) => other.Id == Id;

}