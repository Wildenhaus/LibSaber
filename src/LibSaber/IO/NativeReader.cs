using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace LibSaber.IO;

public unsafe class NativeReader
{

  #region Constants

  const int MIN_BUFFER_SIZE = 1024 * 4; // 4kb buffer

  #endregion

  #region Data Members

  private readonly ReaderImplFuncTable _funcTable;

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

  private static ReaderImplFuncTable GetNativeReaderFuncTable(Endianness dataEndianness)
  {
    bool isHostLittleEndian = BitConverter.IsLittleEndian;
    bool useDefaultImpl = (dataEndianness == Endianness.LittleEndian) == isHostLittleEndian;

    return useDefaultImpl
      ? DefaultReaderImpl.VTable
      : ReverseEndianReaderImpl.VTable;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static void ThrowEndOfStreamException()
    => throw new EndOfStreamException("Reached end of stream while reading.");

  #endregion

}