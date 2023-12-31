﻿using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Network.Interfaces;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Network.Packets; 

public struct PresencePacket : IPlayerPacket {

    public Player? Player { get; set; }
    public IPresence? Presence { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        
        if (Player != null) {
            writer.Put(true);
            writer.Put(Player.Value);
        } else {
            writer.Put(false);
        }
        
        if (Presence != null) {
            writer.Put((byte) Presence.Status);
            writer.Put(Presence);
        } else {
            writer.Put((byte) 0);
        }
        
    }

    public void Deserialize(NetDataReader reader) {
        
        if(reader.GetBool())
            Player = reader.Get<Player>();
        
        var status = reader.GetByte();
        if (status != 0) {
            Presence = (Status) status switch {
                Status.Offline => reader.Get<Presence.Offline>(),
                Status.InMenu => reader.Get<Presence.InMenu>(),
                Status.PlayingMap => reader.Get<Presence.PlayingMap>(),
                Status.PlayingTutorial => reader.Get<Presence.PlayingTutorial>(),
                Status.WatchingReplay => reader.Get<Presence.WatchingReplay>(),
                Status.Afk => reader.Get<Presence.Afk>(),
                _ => Presence
            };
        }
        
    }

}