using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Network.Enums;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Network.Packets; 

/// <summary>
/// <b>If Client receives:</b>
/// <p>- A player of the current lobby received the specified action</p>
/// <br />
/// <b>If Server receives:</b>
/// <p>- A player should be either promoted or kicked</p>
/// <br />
/// <p>The lobby is always the current lobby where the player is in</p>
/// </summary>
public struct LobbyPlayerActionPacket : INetSerializable {

    public LobbyPlayerAction Action { get; set; }
    public Player Player { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put((byte) Action);
        writer.Put(Player);
    }

    public void Deserialize(NetDataReader reader) {
        Action = (LobbyPlayerAction) reader.GetByte();
        Player = reader.Get<Player>();
    }

}