using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BetterBeatSaber.Shared.Serialization; 

public sealed class ByteBuffer : IDisposable {

    public const int DefaultAllocationSize = MaxSize / 2;
    // https://stackoverflow.com/a/18347039
    public const int MaxSize = int.MaxValue - 56; // 2147483591
    
    #region Fields & Properties

    private byte[] _buffer;
    private int _position;
    
    public byte[] Buffer => _buffer;
    public int Position => _position;
    public int Capacity => _buffer.Length;

    public bool FixedSize { get; set; } = false;

    #endregion

    #region Constructors

    public ByteBuffer() : this(DefaultAllocationSize) { }
    
    public ByteBuffer(int size) {
        _buffer = new byte[size];
        _position = 0;
    }
    
    public ByteBuffer(byte[] buffer) {
        _buffer = new byte[buffer.Length];
        _position = 0;
        Array.Copy(buffer, _buffer, buffer.Length);
    }

    #endregion
    
    #region Byte

    public void WriteByte(byte value) {
        EnsureBufferCapacity(sizeof(byte));
        _buffer[_position] = value;
        _position += sizeof(byte);
    }

    public byte ReadByte() {
        EnsureBufferCapacity(sizeof(byte));
        _position += sizeof(byte);
        return _buffer[_position - sizeof(byte)];
    }

    #endregion

    #region SByte

    public void WriteSByte(sbyte value) =>
        WriteByte((byte)value);

    public sbyte ReadSByte() =>
        (sbyte)ReadByte();

    #endregion
    
    #region Bool

    public void WriteBool(bool value) {
        EnsureBufferCapacity(sizeof(bool));
        _buffer[_position] = (byte)(value ? 1 : 0);
        _position++;
    }

    public bool ReadBool() {
        EnsureBufferCapacity(sizeof(bool));
        _position++;
        return _buffer[_position - 1] == 1;
    }

    #endregion

    #region Char

    public unsafe void WriteChar(char value) {
        EnsureBufferCapacity(sizeof(char));
        fixed (byte* pointer = &_buffer[_position]) {
            *(char*)pointer = value;
        }
        _position += sizeof(char);
    }

    public unsafe char ReadChar() {
        EnsureBufferCapacity(sizeof(char));
        _position += sizeof(char);
        fixed (byte* pointer = &_buffer[_position - sizeof(char)]) {
            return *(char*)pointer;
        }
    }

    #endregion
    
    #region Int

    #region Unsigned

    #region 16

    public unsafe void WriteUInt16(ushort value) {
        EnsureBufferCapacity(sizeof(ushort));
        fixed (byte* pointer = &_buffer[_position]) {
            *(ushort*)pointer = value;
        }
        _position += sizeof(ushort);
    }

    public unsafe ushort ReadUInt16() {
        EnsureBufferCapacity(sizeof(ushort));
        _position += sizeof(ushort);
        fixed (byte* pointer = &_buffer[_position - sizeof(ushort)]) {
            return *(ushort*)pointer;
        }
    }

    #endregion

    #region 32

    public unsafe void WriteUInt32(uint value) {
        EnsureBufferCapacity(sizeof(uint));
        fixed (byte* pointer = &_buffer[_position]) {
            *(uint*)pointer = value;
        }
        _position += sizeof(uint);
    }

    public unsafe uint ReadUInt32() {
        EnsureBufferCapacity(sizeof(uint));
        _position += sizeof(uint);
        fixed (byte* pointer = &_buffer[_position - sizeof(uint)]) {
            return *(uint*)pointer;
        }
    }
    
    #endregion

    #region 64

    public unsafe void WriteUInt64(ulong value) {
        EnsureBufferCapacity(sizeof(ulong));
        fixed (byte* pointer = &_buffer[_position]) {
            *(ulong*)pointer = value;
        }
        _position += sizeof(ulong);
    }

    public unsafe ulong ReadUInt64() {
        EnsureBufferCapacity(sizeof(ulong));
        _position += sizeof(ulong);
        fixed (byte* pointer = &_buffer[_position - sizeof(ulong)]) {
            return *(ulong*)pointer;
        }
    }

    #endregion
    
    #endregion

    #region Signed

    #region 16

    public unsafe void WriteInt16(short value) {
        EnsureBufferCapacity(sizeof(short));
        fixed (byte* pointer = &_buffer[_position]) {
            *(short*)pointer = value;
        }
        _position += sizeof(short);
    }

    public unsafe short ReadInt16() {
        EnsureBufferCapacity(sizeof(short));
        _position += sizeof(short);
        fixed (byte* pointer = &_buffer[_position - sizeof(short)]) {
            return *(short*)pointer;
        }
    }

    #endregion

    #region 32

    public unsafe void WriteInt32(int value) {
        EnsureBufferCapacity(sizeof(int));
        fixed (byte* pointer = &_buffer[_position]) {
            *(int*)pointer = value;
        }
        _position += sizeof(int);
    }

    public unsafe int ReadInt32() {
        EnsureBufferCapacity(sizeof(int));
        _position += sizeof(int);
        fixed (byte* pointer = &_buffer[_position - sizeof(int)]) {
            return *(int*)pointer;
        }
    }

    #endregion

    #region 64

    public unsafe void WriteInt64(long value) {
        EnsureBufferCapacity(sizeof(long));
        fixed (byte* pointer = &_buffer[_position]) {
            *(long*)pointer = value;
        }
        _position += sizeof(long);
    }

    public unsafe long ReadInt64() {
        EnsureBufferCapacity(sizeof(long));
        _position += sizeof(long);
        fixed (byte* pointer = &_buffer[_position - sizeof(long)]) {
            return *(long*)pointer;
        }
    }

    #endregion
    
    #endregion
    
    #endregion

    #region Float

    #region 32

    public unsafe void WriteFloat32(float value) {
        EnsureBufferCapacity(sizeof(float));
        fixed (byte* pointer = &_buffer[_position]) {
            *(float*)pointer = value;
        }
        _position += sizeof(float);
    }

    public unsafe float ReadFloat32() {
        EnsureBufferCapacity(sizeof(float));
        _position += sizeof(float);
        fixed (byte* pointer = &_buffer[_position - sizeof(float)]) {
            return *(float*)pointer;
        }
    }

    #endregion

    #region 64

    public unsafe void WriteFloat64(double value) {
        EnsureBufferCapacity(sizeof(double));
        fixed (byte* pointer = &_buffer[_position]) {
            *(double*)pointer = value;
        }
        _position += sizeof(double);
    }

    public unsafe double ReadFloat64() {
        EnsureBufferCapacity(sizeof(double));
        _position += sizeof(double);
        fixed (byte* pointer = &_buffer[_position - sizeof(double)]) {
            return *(double*)pointer;
        }
    }

    #endregion

    #region 128

    public unsafe void WriteFloat128(decimal value) {
        EnsureBufferCapacity(sizeof(decimal));
        fixed (byte* pointer = &_buffer[_position]) {
            *(decimal*)pointer = value;
        }
        _position += sizeof(decimal);
    }

    public unsafe decimal ReadFloat128() {
        EnsureBufferCapacity(sizeof(decimal));
        _position += sizeof(decimal);
        fixed (byte* pointer = &_buffer[_position - sizeof(decimal)]) {
            return *(decimal*)pointer;
        }
    }

    #endregion
    
    #endregion

    #region String

    public void WriteString(string value, Encoding encoding) =>
        WriteByteArray(encoding.GetBytes(value));

    public string ReadString(Encoding encoding) =>
        encoding.GetString(ReadByteArray());

    public void WriteString(string value) =>
        WriteString(value, Encoding.UTF8);

    public string ReadString() =>
        ReadString(Encoding.UTF8);

    #endregion

    #region Guid

    public void WriteGuid(Guid value) =>
        WriteByteArray(value.ToByteArray(), true);

    public Guid ReadGuid() =>
        new(ReadByteArray(16));
    
    #endregion

    #region DateTime

    public void WriteDateTime(DateTime value) =>
        WriteInt64(value.Ticks);

    public DateTime ReadDateTime() =>
        new(ReadInt64());

    #endregion
    
    #region Serializable

    public void Write<T>(T serializable) where T : ISerializable =>
        serializable.Serialize(this);

    public T Read<T>() where T : ISerializable =>
        Serializer.Deserialize<T>(this);

    #endregion

    #region Enum

    public void WriteEnum<T>(T value) where T : Enum {
        var type = typeof(T).GetEnumUnderlyingType();
        WritePrimitive(type, Convert.ChangeType(value, type));
    }

    public T ReadEnum<T>() where T : Enum {
        return (T)ReadPrimitive(typeof(T).GetEnumUnderlyingType());
    }
        
    #endregion

    #region Primivite

    internal static readonly Dictionary<Type, MethodInfo> PrimitiveReadMethods = new() {
        
        { typeof(byte), typeof(ByteBuffer).GetMethod(nameof(ReadByte)) },
        { typeof(sbyte), typeof(ByteBuffer).GetMethod(nameof(ReadSByte)) },
        
        { typeof(bool), typeof(ByteBuffer).GetMethod(nameof(ReadBool)) },
        
        { typeof(ushort), typeof(ByteBuffer).GetMethod(nameof(ReadUInt16)) },
        { typeof(uint), typeof(ByteBuffer).GetMethod(nameof(ReadUInt32)) },
        { typeof(ulong), typeof(ByteBuffer).GetMethod(nameof(ReadUInt64)) },

        { typeof(short), typeof(ByteBuffer).GetMethod(nameof(ReadInt16)) },
        { typeof(int), typeof(ByteBuffer).GetMethod(nameof(ReadInt32)) },
        { typeof(long), typeof(ByteBuffer).GetMethod(nameof(ReadInt64)) },
        
        { typeof(float), typeof(ByteBuffer).GetMethod(nameof(ReadFloat32)) },
        { typeof(double), typeof(ByteBuffer).GetMethod(nameof(ReadFloat64)) },
        { typeof(decimal), typeof(ByteBuffer).GetMethod(nameof(ReadFloat128)) }
        
    };
    
    internal static readonly Dictionary<Type, MethodInfo> PrimitiveWriteMethods = new() {
        
        { typeof(byte), typeof(ByteBuffer).GetMethod(nameof(WriteByte)) },
        { typeof(sbyte), typeof(ByteBuffer).GetMethod(nameof(WriteSByte)) },
        
        { typeof(bool), typeof(ByteBuffer).GetMethod(nameof(WriteBool)) },
        
        { typeof(ushort), typeof(ByteBuffer).GetMethod(nameof(WriteUInt16)) },
        { typeof(uint), typeof(ByteBuffer).GetMethod(nameof(WriteUInt32)) },
        { typeof(ulong), typeof(ByteBuffer).GetMethod(nameof(WriteUInt64)) },

        { typeof(short), typeof(ByteBuffer).GetMethod(nameof(WriteInt16)) },
        { typeof(int), typeof(ByteBuffer).GetMethod(nameof(WriteInt32)) },
        { typeof(long), typeof(ByteBuffer).GetMethod(nameof(WriteInt64)) },
        
        { typeof(float), typeof(ByteBuffer).GetMethod(nameof(WriteFloat32)) },
        { typeof(double), typeof(ByteBuffer).GetMethod(nameof(WriteFloat64)) },
        { typeof(decimal), typeof(ByteBuffer).GetMethod(nameof(WriteFloat128)) }
        
    };

    public void WritePrimitive<T>(T value) =>
        WritePrimitive(typeof(T), value!);
    
    public void WritePrimitive(Type type, object value) {
        
        if (!type.IsPrimitive || !PrimitiveWriteMethods.TryGetValue(type, out var writeMethod) || writeMethod == null)
            throw new Exception("Cannot write this!!!");

        writeMethod.Invoke(this, new[] { value });
        
    }

    public T ReadPrimitive<T>() =>
        (T)ReadPrimitive(typeof(T));

    public object ReadPrimitive(Type type) {
        
        if (!type.IsPrimitive || !PrimitiveReadMethods.TryGetValue(type, out var readMethod) || readMethod == null)
            throw new Exception("Cannot read this!!!");

        return readMethod.Invoke(this, Array.Empty<object>());
        
    }
    
    #endregion
    
    #region Array

    #region Byte Array

    public void WriteByteArray(byte[] array, bool fixedSize = false) {
        EnsureBufferCapacity(array.Length);
        if(!fixedSize)
            WriteInt32(array.Length);
        Array.Copy(array, 0, _buffer, _position, array.Length);
        _position += array.Length;
    }

    public byte[] ReadByteArray(int size = -1) {
        size = size == -1 ? ReadInt32() : size;
        EnsureBufferCapacity(size);
        var array = new byte[size];
        Array.Copy(_buffer, _position, array, 0, size);
        _position += size;
        return array;
    }

    #endregion

    #region Bool Array

    public void WriteBoolArray(bool[] array, bool fixedSize = false) {
        if(!fixedSize)
            WriteInt32(array.Length);
        for (var i = 0; i < array.Length; i++) {
            if (array[i])
                _buffer[_position + i / 8] |= (byte)(1 << (i % 8));
        }
        _position += (array.Length + 7) / 8;
    }

    public bool[] ReadBoolArray(int amount = -1) {
        amount = amount == -1 ? ReadInt32() : amount;
        var array = new bool[amount];
        for (var i = 0; i < amount; i++)
            array[i] = (_buffer[_position + i / 8] & (1 << (i % 8))) != 0;
        return array;
    }

    #endregion
    
    #region String Array

    public void WriteStringArray(string[] array, bool fixedSize = false) {
        if(!fixedSize)
            WriteInt32(array.Length);
        foreach (var str in array)
            WriteString(str);
    }

    public string[] ReadStringArray(int size = -1) {
        size = size == -1 ? ReadInt32() : size;
        var array = new string[size];
        for (var i = 0; i < size; i++)
            array[i] = ReadString();
        return array;
    }

    #endregion

    #region Serializable Array

    public void WriteArray<T>(T[] array, bool fixedSize = false) where T : ISerializable {
        if(!fixedSize)
            WriteInt32(array.Length);
        foreach (var serializable in array)
            Write<T>(serializable);
    }

    public T[] ReadArray<T>(int size = -1) where T : ISerializable {
        size = size == -1 ? ReadInt32() : size;
        var array = new T[size];
        for (var i = 0; i < size; i++)
            array[i] = Read<T>();
        return array;
    }

    #endregion

    #endregion
    
    #region Utilities

    public void EnsureBufferCapacity(int size) {
        if (_position + size > Capacity)
            IncreaseBufferCapacity(size - (Capacity - _position));
    }

    public void IncreaseBufferCapacity(int size) {
        if (!FixedSize)
            Array.Resize(ref _buffer, _position + size);
        else throw new ByteBufferOverflowException();
    }

    public void Clean() {
        var buffer = new byte[_position];
        Array.Copy(_buffer, 0, buffer, 0, _position);
        _buffer = buffer;
    }

    [Obsolete]
    public byte[] ToByteArray(bool clean = true) =>
        ToArray(clean);
    
    public byte[] ToArray(bool clean = true) {
        var buffer = new byte[_position];
        Array.Copy(_buffer, 0, buffer, 0, clean ? _position : _buffer.Length);
        return buffer;
    }
    
    #endregion

    public void Dispose() {
        GC.Collect();
    }

}