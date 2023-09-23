using BetterBeatSaber.Twitch.Shared.Models;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Twitch.Shared.Packets; 

public struct ChatMessagePacket : INetSerializable {

    public ChatMessage ChatMessage { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(ChatMessage);
    }

    public void Deserialize(NetDataReader reader) {
        ChatMessage = reader.Get<ChatMessage>();
    }

}