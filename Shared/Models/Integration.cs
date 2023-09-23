using System;

using BetterBeatSaber.Shared.Enums;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Models; 

public struct Integration : INetSerializable {

    public Guid Id { get; set; }
    public IntegrationType Type { get; set; }
    public string Token { get; set; }
    public string TokenType { get; set; }
    public DateTime ExpiresAt { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(Id.ToString());
        writer.Put((byte) Type);
        writer.Put(Token);
        writer.Put(TokenType);
        writer.Put(ExpiresAt.Ticks);
    }

    public void Deserialize(NetDataReader reader) {
        Id = Guid.Parse(reader.GetString());
        Type = (IntegrationType) reader.GetByte();
        Token = reader.GetString();
        TokenType = reader.GetString();
        ExpiresAt = new DateTime(reader.GetLong());
    }

}