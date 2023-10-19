using BetterBeatSaber.Server.Interfaces;
using BetterBeatSaber.Server.Models;

using LiteNetLib;
using LiteNetLib.Utils;

namespace BetterBeatSaber.Server.Network.Interfaces; 

public interface IServer : IInitializable {

    public NetPacketProcessor PacketProcessor { get; }
    
    public List<IConnection> Connections { get; }
    
    public void SendPacketToPlayerIfConnected<T>(Player player, T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : INetSerializable;

}