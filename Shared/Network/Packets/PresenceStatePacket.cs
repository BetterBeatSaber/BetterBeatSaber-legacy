using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Network.Interfaces;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Network.Packets; 

public struct PresenceStatePacket : IPlayerPacket {

    public Player? Player { get; set; }
    public IPresenceState PresenceState { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        
        if (Player != null) {
            writer.Put(true);
            writer.Put(Player.Value);
        } else {
            writer.Put(false);
        }
        
        writer.Put((byte) PresenceState.Status);
        writer.Put(PresenceState);
        
    }

    public void Deserialize(NetDataReader reader) {
        
        if(reader.GetBool())
            Player = reader.Get<Player>();
        
        var status = reader.GetByte();
        if (status != 0) {
            PresenceState = (Status) status switch {
                Status.PlayingMap => reader.Get<PlayingMapPresenceState>(),
                _ => PresenceState
            };
        }
        
    }

}