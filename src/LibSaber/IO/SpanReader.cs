using System;
using System.Runtime.CompilerServices;

namespace LibSaber.IO;

public ref struct SpanReader
{
  private Span<byte> _span;
  private int _position;

  public SpanReader(Span<byte> span)
  {
    _span = span;
    _position = 0;
  }

  public byte ReadByte()
  {
    byte value = _span[_position];
    _position += sizeof(byte);
    return value;
  }

  public sbyte ReadSByte()
  {
    sbyte value = Unsafe.As<byte, sbyte>(ref _span[_position]);
    _position += sizeof(sbyte);
    return value;
  }

  public short ReadInt16()
  {
    short value = Unsafe.ReadUnaligned<short>(ref _span[_position]);
    _position += sizeof(short);
    return value;
  }

  public ushort ReadUInt16()
  {
    ushort value = Unsafe.ReadUnaligned<ushort>(ref _span[_position]);
    _position += sizeof(ushort);
    return value;
  }

  public int ReadInt32()
  {
    int value = Unsafe.ReadUnaligned<int>(ref _span[_position]);
    _position += sizeof(int);
    return value;
  }

  public uint ReadUInt32()
  {
    uint value = Unsafe.ReadUnaligned<uint>(ref _span[_position]);
    _position += sizeof(uint);
    return value;
  }

  public long ReadInt64()
  {
    long value = Unsafe.ReadUnaligned<long>(ref _span[_position]);
    _position += sizeof(long);
    return value;
  }

  public ulong ReadUInt64()
  {
    ulong value = Unsafe.ReadUnaligned<ulong>(ref _span[_position]);
    _position += sizeof(ulong);
    return value;
  }

  public float ReadFloat32()
  {
    float value = Unsafe.ReadUnaligned<float>(ref _span[_position]);
    _position += sizeof(float);
    return value;
  }

  public double ReadFloat64()
  {
    double value = Unsafe.ReadUnaligned<double>(ref _span[_position]);
    _position += sizeof(double);
    return value;
  }

  public void ResetPosition() => _position = 0;
}