using System.Buffers;
using System.Collections;
using System.Numerics;
using System.Text;

namespace LibSaber.IO
{

  public sealed class NativeReader
  {
    #region Properties

    public Stream BaseStream { get; }

    public Endianness Endianness { get; }

    public long Length => BaseStream.Length;

    public long Position
    {
      get => BaseStream.Position - _bufferLength + _bufferPosition;
      set
      {
        if (value >= BaseStream.Position - _bufferLength && value <= BaseStream.Position)
        {
          _bufferPosition = (int)(value - (BaseStream.Position - _bufferLength));
        }
        else
        {
          BaseStream.Position = value;
          _bufferLength = 0;
          _bufferPosition = 0;
        }
        _position = value;
      }
    }

    private bool NeedsByteOrderSwap { get; }

    private const int DefaultBufferSize = 512*1024; // 512KB buffer
    private byte[] _buffer;
    private int _bufferLength;
    private int _bufferPosition;
    private long _position;

    #endregion

    #region Constructor

    public NativeReader(Stream baseStream, Endianness endianness, int bufferSize = DefaultBufferSize)
    {
      BaseStream = baseStream;
      Endianness = endianness;
      NeedsByteOrderSwap = BitConverter.IsLittleEndian ^ (Endianness == Endianness.LittleEndian);

      _buffer = new byte[bufferSize];
      _bufferLength = 0;
      _bufferPosition = 0;
      _position = BaseStream.Position;
    }

    #endregion

    #region Public Methods

    public int Read(Span<byte> buffer)
    {
      int bytesRead = 0;

      while (buffer.Length > 0)
      {
        if (_bufferPosition >= _bufferLength)
        {
          FillBuffer();
          if (_bufferLength == 0) break;
        }

        int bytesToCopy = Math.Min(buffer.Length, _bufferLength - _bufferPosition);
        _buffer.AsSpan(_bufferPosition, bytesToCopy).CopyTo(buffer);
        _bufferPosition += bytesToCopy;
        buffer = buffer.Slice(bytesToCopy);
        bytesRead += bytesToCopy;
      }

      return bytesRead;
    }

    public Boolean ReadBoolean() => ReadUnmanaged<Boolean>();
    public Byte ReadByte() => ReadUnmanaged<Byte>();
    public SByte ReadSByte() => ReadUnmanaged<SByte>();

    public Int16 ReadInt16() => ReadUnmanaged<Int16>();
    public UInt16 ReadUInt16() => ReadUnmanaged<UInt16>();
    public Int32 ReadInt32() => ReadUnmanaged<Int32>();
    public UInt32 ReadUInt32() => ReadUnmanaged<UInt32>();
    public Int64 ReadInt64() => ReadUnmanaged<Int64>();
    public UInt64 ReadUInt64() => ReadUnmanaged<UInt64>();

    public Half ReadFloat16() => ReadUnmanaged<Half>();
    public Single ReadFloat32() => ReadUnmanaged<Single>();
    public Double ReadFloat64() => ReadUnmanaged<Double>();
    public Decimal ReadFloat128() => ReadUnmanaged<Decimal>();

    public Guid ReadGuid() => ReadUnmanaged<Guid>();

    public string ReadFixedLengthString(int length)
    {
      byte[] buffer = ArrayPool<byte>.Shared.Rent(length);
      Read(buffer.AsSpan().Slice(0, length));
      var result = Encoding.ASCII.GetString(buffer,0, length);
      ArrayPool<byte>.Shared.Return(buffer);
      return result;
    }

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

    public string ReadLengthPrefixedString16() => ReadFixedLengthString(ReadUnmanaged<Int16>());
    public string ReadLengthPrefixedString32() => ReadFixedLengthString(ReadUnmanaged<Int32>());

    public Vector3 ReadVector3() => new Vector3(ReadFloat32(), ReadFloat32(), ReadFloat32());

    public Vector4 ReadVector4() => new Vector4(ReadFloat32(), ReadFloat32(), ReadFloat32(), ReadFloat32());

    public Matrix4x4 ReadMatrix3x3() => new Matrix4x4(
        ReadFloat32(), ReadFloat32(), ReadFloat32(), 0,
        ReadFloat32(), ReadFloat32(), ReadFloat32(), 0,
        ReadFloat32(), ReadFloat32(), ReadFloat32(), 0,
        0, 0, 0, 1
    );

    public Matrix4x4 ReadMatrix4x4() => new Matrix4x4(
        ReadFloat32(), ReadFloat32(), ReadFloat32(), ReadFloat32(),
        ReadFloat32(), ReadFloat32(), ReadFloat32(), ReadFloat32(),
        ReadFloat32(), ReadFloat32(), ReadFloat32(), ReadFloat32(),
        ReadFloat32(), ReadFloat32(), ReadFloat32(), ReadFloat32()
    );

    public BitArray ReadBitArray(int bitCount)
    {
      int readLen = (int)Math.Ceiling(bitCount / 8f);
      byte[] buffer = new byte[readLen];
      Read(buffer.AsSpan());
      return new BitArray(buffer);
    }

    public byte PeekByte()
    {
      var b = ReadByte();
      _bufferPosition -= 1;
      _position -= 1;
      return b;
    }

    public void Seek(long offset, SeekOrigin origin = SeekOrigin.Begin)
    {
      BaseStream.Seek(offset, origin);
      _bufferPosition = 0;
      _bufferLength = 0;
      _position = BaseStream.Position;
    }

    #endregion

    #region Private Methods

    private void FillBuffer()
    {
      _bufferPosition = 0;
      _bufferLength = BaseStream.Read(_buffer, 0, _buffer.Length);
      _position += _bufferLength;
    }

    internal unsafe T ReadUnmanaged<T>() where T : unmanaged
    {
      var alloc = stackalloc byte[sizeof(T)];
      var buffer = new Span<byte>(alloc, sizeof(T));
      Read(buffer);

      if (NeedsByteOrderSwap)
        buffer.Reverse();

      return *(T*)alloc;
    }

    #endregion
  }

}
