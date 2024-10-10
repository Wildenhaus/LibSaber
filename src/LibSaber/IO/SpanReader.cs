using System.Numerics;
using System.Runtime.CompilerServices;

namespace LibSaber.IO;

public unsafe ref struct SpanReader
{

  private Span<byte> _buffer;
  private int _bufferPosition;
  private readonly ReaderImplFuncTable _funcTable;

  public int Position => _bufferPosition;

  public SpanReader(Span<byte> buffer, Endianness endianness = Endianness.LittleEndian)
  {
    _buffer = buffer;
    _bufferPosition = 0;

    _funcTable = GetNativeReaderFuncTable(endianness);
  }

  public SpanReader GetSpanReader(int length)
  {
    var reader = new SpanReader(_buffer.Slice(_bufferPosition, length));
    _bufferPosition += length;
    return reader;
  }

  #region Read Methods

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public bool ReadBoolean()
  {
    bool value = Unsafe.ReadUnaligned<bool>(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(bool);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public byte ReadByte()
  {
    byte value = _buffer[_bufferPosition];
    _bufferPosition += sizeof(byte);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public sbyte ReadSByte()
  {
    sbyte value = Unsafe.ReadUnaligned<sbyte>(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(sbyte);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public short ReadInt16()
  {
    short value = _funcTable.ReadInt16(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(short);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public ushort ReadUInt16()
  {
    ushort value = _funcTable.ReadUInt16(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(ushort);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public int ReadInt32()
  {
    int value = _funcTable.ReadInt32(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(int);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public uint ReadUInt32()
  {
    uint value = _funcTable.ReadUInt32(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(uint);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public long ReadInt64()
  {
    long value = _funcTable.ReadInt64(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(long);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public ulong ReadUInt64()
  {
    ulong value = _funcTable.ReadUInt64(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(ulong);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public float ReadFloat32()
  {
    float value = _funcTable.ReadFloat32(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(float);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public double ReadFloat64()
  {
    double value = _funcTable.ReadFloat64(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(double);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public Guid ReadGuid()
  {
    Guid value = _funcTable.ReadGuid(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(Guid);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public Vector3 ReadVector3()
  {
    Vector3 value = _funcTable.ReadVector3(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(Vector3);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public Vector4 ReadVector4()
  {
    Vector4 value = _funcTable.ReadVector4(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(Vector4);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public Matrix4x4 ReadMatrix3x3()
  {
    const int SIZE = sizeof(float) * 3 * 3;

    Matrix4x4 value = _funcTable.ReadMatrix3x3(ref _buffer[_bufferPosition]);
    _bufferPosition += SIZE;
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public Matrix4x4 ReadMatrix4x4()
  {
    Matrix4x4 value = _funcTable.ReadMatrix4x4(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(Matrix4x4);
    return value;
  }

  #endregion

  #region Private Methods

  private static ReaderImplFuncTable GetNativeReaderFuncTable(Endianness dataEndianness)
  {
    bool isHostLittleEndian = BitConverter.IsLittleEndian;
    bool useDefaultImpl = (dataEndianness == Endianness.LittleEndian) == isHostLittleEndian;

    return useDefaultImpl
      ? DefaultReaderImpl.VTable
      : ReverseEndianReaderImpl.VTable;
  }

  #endregion

}
