using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using BP = System.Buffers.Binary.BinaryPrimitives;

namespace LibSaber.IO;

#region NativeReaderImpl

internal unsafe struct ReaderImplFuncTable
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

internal interface IReaderImpl
{

  static abstract ReaderImplFuncTable VTable { get; }

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

internal unsafe struct DefaultReaderImpl : IReaderImpl
{

  public static ReaderImplFuncTable VTable { get; } =
    new ReaderImplFuncTable
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

internal unsafe struct ReverseEndianReaderImpl : IReaderImpl
{

  public static ReaderImplFuncTable VTable { get; } =
    new ReaderImplFuncTable
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