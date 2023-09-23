using System;
using System.Collections.Generic;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Models;

public struct Lobby : INetSerializable {

    public Guid Id { get; set; }
    public string Code { get; set; }
    public Player Owner { get; set; }
    public byte MaxPlayers { get; set; }
    public List<Player> Players { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(Id.ToString());
        writer.Put(Code);
        writer.Put(Owner);
        writer.Put(MaxPlayers);
        writer.Put(Players.Count);
        foreach (var player in Players)
            writer.Put(player);
    }

    public void Deserialize(NetDataReader reader) {
        Id = Guid.Parse(reader.GetString());
        Code = reader.GetString();
        Owner = reader.Get<Player>();
        MaxPlayers = reader.GetByte();
        Players = new List<Player>(byte.MaxValue);
        var amount = reader.GetInt();
        for(var i = 0; i < amount; i++)
            Players.Add(reader.Get<Player>());
    }

}