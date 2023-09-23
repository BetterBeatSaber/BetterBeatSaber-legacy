using BetterBeatSaber.Shared.Enums;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Network.Interfaces; 

public interface ILobby : INetSerializable {

    public MultiplayerService Service { get; }

}