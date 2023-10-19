namespace BetterBeatSaber.Shared.Serialization; 

public interface ISerializable {

    public void Serialize(ByteBuffer buffer);
    public void Deserialize(ByteBuffer buffer);

}