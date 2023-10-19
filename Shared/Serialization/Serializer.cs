using System;

namespace BetterBeatSaber.Shared.Serialization; 

public static class Serializer {

    public static byte[] Serialize<T>(T serializable) where T : ISerializable {
        var buffer = new ByteBuffer();
        serializable.Serialize(buffer);
        return buffer.Buffer;
    }

    public static T Deserialize<T>(byte[] array) where T : ISerializable =>
        Deserialize<T>(new ByteBuffer(array));
    
    public static T Deserialize<T>(ByteBuffer buffer) where T : ISerializable {
        var serializable = Activator.CreateInstance<T>();
        serializable.Deserialize(buffer);
        return serializable;
    }

}