using System.Buffers;
using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using BP = System.Buffers.Binary.BinaryPrimitives;

namespace LibSaber.IO;

public unsafe class NativeReader
{

  #region Constants

  const int MIN_BUFFER_SIZE = 1024 * 4; // 4kb buffer

  #endregion

  #region Data Members

  private readonly NativeReaderImplFuncTable _funcTable;

  private readonly Stream _stream;
  private readonly byte[] _buffer;
  private readonly int _bufferCapacity;
  private int _bufferedBytes;
  private int _bufferPosition;
  private long _streamPosition;

  #endregion

  #region Properties

  public Stream BaseStream => _stream;

  public long Position
  {
    get => _streamPosition - (_bufferedBytes - _bufferPosition);
    set => Seek(value, SeekOrigin.Begin);
  }

  public long Length => _stream.Length;

  #endregion

  #region Constructor

  public NativeReader(Stream stream, Endianness endianness = Endianness.LittleEndian, int bufferSize = MIN_BUFFER_SIZE)
  {
    _stream = stream ?? throw new ArgumentNullException(nameof(stream));
    _buffer = new byte[bufferSize];
    _bufferCapacity = _buffer.Length;
    _bufferedBytes = 0;
    _bufferPosition = 0;
    _streamPosition = stream.Position;

    _funcTable = GetNativeReaderFuncTable(endianness);
  }

  #endregion

  #region Public Methods

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public int Read(Span<byte> buffer)
  {
    int totalBytesRead = 0;
    int bytesToRead = buffer.Length;

    while (bytesToRead > 0)
    {
      if (_bufferPosition < _bufferedBytes)
      {
        int bytesAvailable = _bufferedBytes - _bufferPosition;
        int bytesFromBuffer = Math.Min(bytesAvailable, bytesToRead);

        _buffer.AsSpan(_bufferPosition, bytesFromBuffer).CopyTo(buffer.Slice(totalBytesRead));
        _bufferPosition += bytesFromBuffer;
        totalBytesRead += bytesFromBuffer;
        bytesToRead -= bytesFromBuffer;

        if (bytesToRead == 0)
          break;
      }

      FillBuffer(Math.Min(bytesToRead, _bufferCapacity));
    }

    return totalBytesRead;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public bool ReadBoolean()
  {
    FillBuffer(sizeof(bool));
    bool value = Unsafe.ReadUnaligned<bool>(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(bool);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public byte ReadByte()
  {
    FillBuffer(sizeof(byte));
    byte value = Unsafe.ReadUnaligned<byte>(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(byte);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public sbyte ReadSByte()
  {
    FillBuffer(sizeof(sbyte));
    sbyte value = Unsafe.ReadUnaligned<sbyte>(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(sbyte);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public short ReadInt16()
  {
    FillBuffer(sizeof(short));
    short value = _funcTable.ReadInt16(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(short);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public ushort ReadUInt16()
  {
    FillBuffer(sizeof(ushort));
    ushort value = _funcTable.ReadUInt16(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(ushort);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public int ReadInt32()
  {
    FillBuffer(sizeof(int));
    int value = _funcTable.ReadInt32(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(int);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public uint ReadUInt32()
  {
    FillBuffer(sizeof(uint));
    uint value = _funcTable.ReadUInt32(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(uint);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public long ReadInt64()
  {
    FillBuffer(sizeof(long));
    long value = _funcTable.ReadInt64(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(long);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public ulong ReadUInt64()
  {
    FillBuffer(sizeof(ulong));
    ulong value = _funcTable.ReadUInt64(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(ulong);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public float ReadFloat32()
  {
    FillBuffer(sizeof(float));
    float value = _funcTable.ReadFloat32(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(float);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public double ReadFloat64()
  {
    FillBuffer(sizeof(double));
    double value = _funcTable.ReadFloat64(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(double);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public Guid ReadGuid()
  {
    FillBuffer(sizeof(Guid));
    Guid value = _funcTable.ReadGuid(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(Guid);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public string ReadFixedLengthString(int length)
  {
    byte[] buffer = new byte[length];
    Read(buffer);
    var result = Encoding.ASCII.GetString(buffer, 0, length);
    return result;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public string ReadNullTerminatedString()
  {
    var sb = new StringBuilder();
    byte c;
    while ((c = ReadByte()) != 0)
    {
      sb.Append((char)c);
    }

    return sb.ToString();
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public string ReadLengthPrefixedString16()
  {
    var length = ReadInt16();
    return ReadFixedLengthString(length);
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public string ReadLengthPrefixedString32()
  {
    var length = ReadInt32();
    return ReadFixedLengthString(length);
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public Vector3 ReadVector3()
  {
    FillBuffer(sizeof(Vector3));
    Vector3 value = _funcTable.ReadVector3(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(Vector3);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public Vector4 ReadVector4()
  {
    FillBuffer(sizeof(Vector4));
    Vector4 value = _funcTable.ReadVector4(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(Vector4);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public Matrix4x4 ReadMatrix3x3()
  {
    const int SIZE = sizeof(float) * 3 * 3;

    FillBuffer(SIZE);
    Matrix4x4 value = _funcTable.ReadMatrix3x3(ref _buffer[_bufferPosition]);
    _bufferPosition += SIZE;
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public Matrix4x4 ReadMatrix4x4()
  {
    FillBuffer(sizeof(Matrix4x4));
    Matrix4x4 value = _funcTable.ReadMatrix4x4(ref _buffer[_bufferPosition]);
    _bufferPosition += sizeof(Matrix4x4);
    return value;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public BitArray ReadBitArray(int bitCount)
  {
    var readLen = (int)Math.Ceiling(bitCount / 8f);
    var buffer = new byte[readLen];
    Read(buffer);

    return new BitArray(buffer);
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public byte PeekByte()
  {
    var b = ReadByte();
    Position--;
    return b;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public T ReadUnmanaged<T>()
    where T : unmanaged
  {
    var alloc = stackalloc byte[sizeof(T)];
    var buffer = new Span<byte>(alloc, sizeof(T));

    Read(buffer);
    return *(T*)alloc;
  }

  #endregion

  #region Private Methods

  private void Seek(long offset, SeekOrigin origin)
  {
    long targetPosition = _streamPosition - (_bufferedBytes - _bufferPosition);
    long newPosition = origin switch
    {
      SeekOrigin.Begin => offset,
      SeekOrigin.Current => targetPosition + offset,
      SeekOrigin.End => _stream.Length + offset,
      _ => offset
    };

    ArgumentOutOfRangeException.ThrowIfLessThan(newPosition, 0, nameof(newPosition));
    ArgumentOutOfRangeException.ThrowIfGreaterThan(newPosition, _stream.Length, nameof(newPosition));

    if (newPosition != targetPosition)
    {
      _stream.Position = newPosition;
      _streamPosition = newPosition;
      _bufferedBytes = 0;
      _bufferPosition = 0;
    }
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  private void FillBuffer(int minimumBytes)
  {
    if (_bufferedBytes - _bufferPosition >= minimumBytes)
      return;

    if (_bufferPosition > 0)
    {
      Buffer.BlockCopy(_buffer, _bufferPosition, _buffer, 0, _bufferedBytes - _bufferPosition);
      _bufferedBytes -= _bufferPosition;
      _bufferPosition = 0;
    }

    while (_bufferedBytes < minimumBytes)
    {
      int bytesRead = _stream.Read(_buffer.AsSpan(_bufferedBytes));
      if (bytesRead == 0)
        ThrowEndOfStreamException();
      _bufferedBytes += bytesRead;
      _streamPosition += bytesRead;
    }
  }

  private static NativeReaderImplFuncTable GetNativeReaderFuncTable(Endianness dataEndianness)
  {
    bool isHostLittleEndian = BitConverter.IsLittleEndian;
    bool useDefaultImpl = (dataEndianness == Endianness.LittleEndian) == isHostLittleEndian;

    return useDefaultImpl
      ? DefaultNativeReaderImpl.VTable
      : ReverseEndianNativeReaderImpl.VTable;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static void ThrowEndOfStreamException()
    => throw new EndOfStreamException("Reached end of stream while reading.");

  #endregion

}

#region NativeReaderImpl

internal unsafe struct NativeReaderImplFuncTable
{

  public delegate*<ref byte, short> ReadInt16;
  public delegate*<ref byte, ushort> ReadUInt16;
  public delegate*<ref byte, int> ReadInt32;
  public delegate*<ref byte, uint> ReadUInt32;
  public delegate*<ref byte, long> ReadInt64;
  public delegate*<ref byte, ulong> ReadUInt64;
  public delegate*<ref byte, float> ReadFloat32;
  public delegate*<ref byte, double> ReadFloat64;
  public delegate*<ref byte, Guid> ReadGuid;

  public delegate*<ref byte, Vector3> ReadVector3;
  public delegate*<ref byte, Vector4> ReadVector4;
  public delegate*<ref byte, Matrix4x4> ReadMatrix3x3;
  public delegate*<ref byte, Matrix4x4> ReadMatrix4x4;
}

internal interface INativeReaderImpl
{

  static abstract NativeReaderImplFuncTable VTable { get; }

  static abstract short ReadInt16(ref byte source);
  static abstract ushort ReadUInt16(ref byte source);
  static abstract int ReadInt32(ref byte source);
  static abstract uint ReadUInt32(ref byte source);
  static abstract long ReadInt64(ref byte source);
  static abstract ulong ReadUInt64(ref byte source);
  static abstract float ReadFloat32(ref byte source);
  static abstract double ReadFloat64(ref byte source);
  static abstract Guid ReadGuid(ref byte source);

  static abstract Vector3 ReadVector3(ref byte source);
  static abstract Vector4 ReadVector4(ref byte source);
  static abstract Matrix4x4 ReadMatrix3x3(ref byte source);
  static abstract Matrix4x4 ReadMatrix4x4(ref byte source);

}

internal unsafe struct DefaultNativeReaderImpl : INativeReaderImpl
{

  public static NativeReaderImplFuncTable VTable { get; } =
    new NativeReaderImplFuncTable
    {
      ReadInt16 = &ReadInt16,
      ReadUInt16 = &ReadUInt16,
      ReadInt32 = &ReadInt32,
      ReadUInt32 = &ReadUInt32,
      ReadInt64 = &ReadInt64,
      ReadUInt64 = &ReadUInt64,
      ReadFloat32 = &ReadFloat32,
      ReadFloat64 = &ReadFloat64,
      ReadGuid = &ReadGuid,

      ReadVector3 = &ReadVector3,
      ReadVector4 = &ReadVector4,
      ReadMatrix3x3 = &ReadMatrix3x3,
      ReadMatrix4x4 = &ReadMatrix4x4,
    };

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static short ReadInt16(ref byte source)
    => Unsafe.ReadUnaligned<short>(in source);

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static ushort ReadUInt16(ref byte source)
    => Unsafe.ReadUnaligned<ushort>(in source);

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static int ReadInt32(ref byte source)
    => Unsafe.ReadUnaligned<int>(in source);

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static uint ReadUInt32(ref byte source)
    => Unsafe.ReadUnaligned<uint>(in source);

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static long ReadInt64(ref byte source)
    => Unsafe.ReadUnaligned<long>(in source);

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static ulong ReadUInt64(ref byte source)
    => Unsafe.ReadUnaligned<ulong>(in source);

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static float ReadFloat32(ref byte source)
    => Unsafe.ReadUnaligned<float>(in source);

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static double ReadFloat64(ref byte source)
    => Unsafe.ReadUnaligned<double>(in source);

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static Guid ReadGuid(ref byte source)
    => Unsafe.ReadUnaligned<Guid>(in source);

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static Vector3 ReadVector3(ref byte source)
    => Unsafe.ReadUnaligned<Vector3>(in source);

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static Vector4 ReadVector4(ref byte source)
    => Unsafe.ReadUnaligned<Vector4>(in source);

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static Matrix4x4 ReadMatrix3x3(ref byte source)
  {
    var a = ReadVector3(ref source);
    var b = ReadVector3(ref Unsafe.Add(ref source, 12));
    var c = ReadVector3(ref Unsafe.Add(ref source, 24));

#pragma warning disable format
    return new Matrix4x4(a.X, a.Y, a.Z, 0,
                         b.X, b.Y, b.Z, 0,
                         c.X, c.Y, c.Z, 0,
                           0,   0,   0, 1);
#pragma warning restore format
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static Matrix4x4 ReadMatrix4x4(ref byte source)
    => Unsafe.ReadUnaligned<Matrix4x4>(in source);

}

internal unsafe struct ReverseEndianNativeReaderImpl : INativeReaderImpl
{

  public static NativeReaderImplFuncTable VTable { get; } =
    new NativeReaderImplFuncTable
    {
      ReadInt16 = &ReadInt16,
      ReadUInt16 = &ReadUInt16,
      ReadInt32 = &ReadInt32,
      ReadUInt32 = &ReadUInt32,
      ReadInt64 = &ReadInt64,
      ReadUInt64 = &ReadUInt64,
      ReadFloat32 = &ReadFloat32,
      ReadFloat64 = &ReadFloat64,
      ReadGuid = &ReadGuid,

      ReadVector3 = &ReadVector3,
      ReadVector4 = &ReadVector4,
      ReadMatrix3x3 = &ReadMatrix3x3,
      ReadMatrix4x4 = &ReadMatrix4x4,
    };

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static short ReadInt16(ref byte source)
    => BP.ReverseEndianness(Unsafe.ReadUnaligned<short>(in source));

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static ushort ReadUInt16(ref byte source)
    => BP.ReverseEndianness(Unsafe.ReadUnaligned<ushort>(in source));

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static int ReadInt32(ref byte source)
    => BP.ReverseEndianness(Unsafe.ReadUnaligned<int>(in source));

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static uint ReadUInt32(ref byte source)
    => BP.ReverseEndianness(Unsafe.ReadUnaligned<uint>(in source));

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static long ReadInt64(ref byte source)
    => BP.ReverseEndianness(Unsafe.ReadUnaligned<long>(in source));

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static ulong ReadUInt64(ref byte source)
    => BP.ReverseEndianness(Unsafe.ReadUnaligned<ulong>(in source));

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static float ReadFloat32(ref byte source)
    => Unsafe.BitCast<uint, float>(BP.ReverseEndianness(Unsafe.ReadUnaligned<uint>(in source)));

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static double ReadFloat64(ref byte source)
    => Unsafe.BitCast<ulong, double>(BP.ReverseEndianness(Unsafe.ReadUnaligned<ulong>(in source)));

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static Guid ReadGuid(ref byte source)
  {
    int a = BP.ReverseEndianness(Unsafe.ReadUnaligned<int>(in source));
    short b = BP.ReverseEndianness(Unsafe.ReadUnaligned<short>(ref Unsafe.Add(ref source, 4)));
    short c = BP.ReverseEndianness(Unsafe.ReadUnaligned<short>(ref Unsafe.Add(ref source, 6)));
    long dK = BP.ReverseEndianness(Unsafe.ReadUnaligned<long>(ref Unsafe.Add(ref source, 8)));

    byte d = (byte)((dK >> 0) & 0xFF);
    byte e = (byte)((dK >> 8) & 0xFF);
    byte f = (byte)((dK >> 16) & 0xFF);
    byte g = (byte)((dK >> 24) & 0xFF);
    byte h = (byte)((dK >> 32) & 0xFF);
    byte i = (byte)((dK >> 40) & 0xFF);
    byte j = (byte)((dK >> 48) & 0xFF);
    byte k = (byte)((dK >> 56) & 0xFF);

    return new Guid(a, b, c, d, e, f, g, h, i, j, k);
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static Vector3 ReadVector3(ref byte source)
  {
    var shuffleMask = Vector128.Create((byte)03, (byte)02, (byte)01, (byte)00,
                                       (byte)07, (byte)06, (byte)05, (byte)04,
                                       (byte)11, (byte)10, (byte)09, (byte)08,
                                       (byte)00, (byte)00, (byte)00, (byte)00);

    fixed (byte* ptr = &source)
    {
      Vector128<byte> vector = Sse2.LoadVector128(ptr);
      Vector128<byte> reversedVector = Ssse3.Shuffle(vector, shuffleMask);
      Sse2.Store(ptr, reversedVector);
    }

    return Unsafe.ReadUnaligned<Vector3>(ref source);
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static Vector4 ReadVector4(ref byte source)
  {
    var shuffleMask = Vector128.Create((byte)03, (byte)02, (byte)01, (byte)00,
                                       (byte)07, (byte)06, (byte)05, (byte)04,
                                       (byte)11, (byte)10, (byte)09, (byte)08,
                                       (byte)15, (byte)14, (byte)13, (byte)12);

    fixed (byte* ptr = &source)
    {
      Vector128<byte> vector = Sse2.LoadVector128(ptr);
      Vector128<byte> reversedVector = Ssse3.Shuffle(vector, shuffleMask);
      Sse2.Store(ptr, reversedVector);
    }

    return Unsafe.ReadUnaligned<Vector4>(ref source);
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static Matrix4x4 ReadMatrix3x3(ref byte source)
  {
    Matrix4x4 result = default;

    fixed (byte* ptr = &source)
    {
      Vector128<float> row1 = Sse.LoadVector128((float*)ptr);
      Vector128<float> row2 = Sse.LoadVector128((float*)(ptr + 12));
      Vector128<float> row3 = Sse.LoadVector128((float*)(ptr + 24));

      result.M11 = row1.GetElement(0); result.M12 = row1.GetElement(1); result.M13 = row1.GetElement(2); result.M14 = 0;
      result.M21 = row2.GetElement(0); result.M22 = row2.GetElement(1); result.M23 = row2.GetElement(2); result.M24 = 0;
      result.M31 = row3.GetElement(0); result.M32 = row3.GetElement(1); result.M33 = row3.GetElement(2); result.M34 = 0;

      result.M41 = 0;
      result.M42 = 0;
      result.M43 = 0;
      result.M44 = 1;
    }

    return result;
  }

  [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
  public static Matrix4x4 ReadMatrix4x4(ref byte source)
  {
    Matrix4x4 result = default;

    fixed (byte* ptr = &source)
    {
      Vector128<float> row1 = Sse.LoadVector128((float*)ptr);
      Vector128<float> row2 = Sse.LoadVector128((float*)(ptr + 16));
      Vector128<float> row3 = Sse.LoadVector128((float*)(ptr + 32));
      Vector128<float> row4 = Sse.LoadVector128((float*)(ptr + 48));

      result.M11 = row1.GetElement(0); result.M12 = row1.GetElement(1); result.M13 = row1.GetElement(2); result.M14 = row1.GetElement(3);
      result.M21 = row2.GetElement(0); result.M22 = row2.GetElement(1); result.M23 = row2.GetElement(2); result.M24 = row2.GetElement(3);
      result.M31 = row3.GetElement(0); result.M32 = row3.GetElement(1); result.M33 = row3.GetElement(2); result.M34 = row3.GetElement(3);
      result.M41 = row4.GetElement(0); result.M42 = row4.GetElement(1); result.M43 = row4.GetElement(2); result.M44 = row4.GetElement(3);
    }

    return result;
  }


}

#endregion