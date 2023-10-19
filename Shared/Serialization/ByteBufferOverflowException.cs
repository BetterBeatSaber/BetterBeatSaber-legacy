using System;

namespace BetterBeatSaber.Shared.Serialization; 

public sealed class ByteBufferOverflowException : Exception {

    public ByteBufferOverflowException() : base("Cannot read or write more because the ByteBuffer has a fixed size") { }

}