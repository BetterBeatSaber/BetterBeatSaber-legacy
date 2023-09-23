using LiteNetLib.Utils;

namespace BetterBeatSaber.Twitch.Shared.Models; 

public struct ChatMessage : INetSerializable {

    public string Id { get; set; }
    public string Channel { get; set; }
    public User User { get; set; }
    public string Message { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(Id);
        writer.Put(Channel);
        writer.Put(User);
        writer.Put(Message);
    }

    public void Deserialize(NetDataReader reader) {
        Id = reader.GetString();
        Channel = reader.GetString();
        User = reader.Get<User>();
        Message = reader.GetString();
    }

}