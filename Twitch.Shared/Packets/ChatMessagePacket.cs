using BetterBeatSaber.Twitch.Shared.Models;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Twitch.Shared.Packets; 

public struct ChatMessagePacket : INetSerializable {

    public ChatMessage ChatMessage { get; set; }
    public string? TextToSpeechToken { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(ChatMessage);
        writer.Put(TextToSpeechToken != null);
        if (TextToSpeechToken != null)
            writer.Put(TextToSpeechToken);
    }

    public void Deserialize(NetDataReader reader) {
        ChatMessage = reader.Get<ChatMessage>();
        if (reader.GetBool())
            TextToSpeechToken = reader.GetString();
    }

}