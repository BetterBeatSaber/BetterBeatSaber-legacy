using BetterBeatSaber.Shared.Models;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Network.Interfaces; 

public interface IPlayerPacket : INetSerializable {

    public Player? Player { get; set; }

}