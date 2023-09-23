using BetterBeatSaber.Shared.Enums;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Network.Interfaces; 

public interface IPresenceState : INetSerializable {

    public Status Status { get; }

}