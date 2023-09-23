using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Network.Enums;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Network.Packets;

/// <summary>
/// <b>If Client receives:</b>
/// <p>- The response for the client if created, joined or left a lobby</p>
/// <p>- If the lobby is null then an error occurred</p>
/// <br />
/// <b>If Server receives:</b>
/// <p>- The player either wants to create, join or leave a lobby</p>
/// <p>- If the player wants to create a lobby the lobby code can be null</p>
/// <p>- Lobby will be always null</p>
/// </summary>
public struct LobbyActionPacket : INetSerializable {

    public LobbyAction Action { get; set; }
    public string? LobbyCode { get; set; }
    public Lobby? Lobby { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put((byte) Action);
        writer.Put(LobbyCode ?? string.Empty);
        writer.Put(Lobby != null);
        if(Lobby != null)
            writer.Put(Lobby.Value);
    }

    public void Deserialize(NetDataReader reader) {
        Action = (LobbyAction) reader.GetByte();
        LobbyCode = reader.GetString();
        if (reader.GetBool())
            Lobby = reader.Get<Lobby>();
    }

}