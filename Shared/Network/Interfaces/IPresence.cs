using BetterBeatSaber.Shared.Enums;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Network.Interfaces; 

public interface IPresence : INetSerializable {

    public Status Status { get; }

}