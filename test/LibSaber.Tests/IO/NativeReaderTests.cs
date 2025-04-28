using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LibSaber.IO.Tests;

public class NativeReaderTests : NativeReaderTestsBase
{

  #region Tests

  [Fact]
  public void NativeReader_reads_expected_values_primitive()
  {
    const int ELEMENT_COUNT = 1_000_000;

    RunExpectedValuesTestPrimitive<bool>(ELEMENT_COUNT, false, static reader => reader.ReadBoolean());
    RunExpectedValuesTestPrimitive<byte>(ELEMENT_COUNT, false, static reader => reader.ReadByte());
    RunExpectedValuesTestPrimitive<sbyte>(ELEMENT_COUNT, false, static reader => reader.ReadSByte());
    RunExpectedValuesTestPrimitive<short>(ELEMENT_COUNT, false, static reader => reader.ReadInt16());
    RunExpectedValuesTestPrimitive<ushort>(ELEMENT_COUNT, false, static reader => reader.ReadUInt16());
    RunExpectedValuesTestPrimitive<int>(ELEMENT_COUNT, false, static reader => reader.ReadInt32());
    RunExpectedValuesTestPrimitive<uint>(ELEMENT_COUNT, false, static reader => reader.ReadUInt32());
    RunExpectedValuesTestPrimitive<long>(ELEMENT_COUNT, false, static reader => reader.ReadInt64());
    RunExpectedValuesTestPrimitive<ulong>(ELEMENT_COUNT, false, static reader => reader.ReadUInt64());
    RunExpectedValuesTestPrimitive<float>(ELEMENT_COUNT, false, static reader => reader.ReadFloat32());
    RunExpectedValuesTestPrimitive<double>(ELEMENT_COUNT, false, static reader => reader.ReadFloat64());
  }

  [Fact]
  public void NativeReader_reads_expected_values_primitive_reverse_endianness()
  {
    const int ELEMENT_COUNT = 1_000_000;

    RunExpectedValuesTestPrimitive<bool>(ELEMENT_COUNT, true, static reader => reader.ReadBoolean());
    RunExpectedValuesTestPrimitive<byte>(ELEMENT_COUNT, true, static reader => reader.ReadByte());
    RunExpectedValuesTestPrimitive<sbyte>(ELEMENT_COUNT, true, static reader => reader.ReadSByte());
    RunExpectedValuesTestPrimitive<short>(ELEMENT_COUNT, true, static reader => reader.ReadInt16());
    RunExpectedValuesTestPrimitive<ushort>(ELEMENT_COUNT, true, static reader => reader.ReadUInt16());
    RunExpectedValuesTestPrimitive<int>(ELEMENT_COUNT, true, static reader => reader.ReadInt32());
    RunExpectedValuesTestPrimitive<uint>(ELEMENT_COUNT, true, static reader => reader.ReadUInt32());
    RunExpectedValuesTestPrimitive<long>(ELEMENT_COUNT, true, static reader => reader.ReadInt64());
    RunExpectedValuesTestPrimitive<ulong>(ELEMENT_COUNT, true, static reader => reader.ReadUInt64());
    RunExpectedValuesTestPrimitive<float>(ELEMENT_COUNT, true, static reader => reader.ReadFloat32());
    RunExpectedValuesTestPrimitive<double>(ELEMENT_COUNT, true, static reader => reader.ReadFloat64());
  }

  #endregion

  #region Helpers

  #region Primitive

  private void RunExpectedValuesTestPrimitive<T>(int elementCount, bool swapEndianness, Func<NativeReader, T> readFunc)
    where T : unmanaged
  {
    var seed = Guid.NewGuid().GetHashCode();
    var stream = CreateMemoryStreamPrimitive<T>(elementCount, seed, swapEndianness);
    var reader = new NativeReader(stream, swapEndianness ? ReverseEndianness : HostEndianness);

    foreach(var generatedElement in GenerateRandomElementsPrimitive<T>(elementCount, seed))
    {
      var readElement = readFunc(reader);
      Assert.True(generatedElement.Equals(readElement));
    }

    Assert.True(stream.Position == stream.Length);
  }


  private MemoryStream CreateMemoryStreamPrimitive<T>(int elementCount, int seed, bool swapEndianness)
    where T : unmanaged
  {
    var stream = new MemoryStream();
    foreach(var generatedElement in GenerateRandomElementsPrimitive<T>(elementCount, seed))
    {
      T elem = generatedElement;
      var bytes = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref elem, 1));

      if (swapEndianness)
        bytes.Reverse();

      stream.Write(bytes);
    }

    stream.Position = 0;
    return stream;
  }

  private IEnumerable<T> GenerateRandomElementsPrimitive<T>(int elementCount, int seed)
    where T : unmanaged
  {
    var elementSize = Unsafe.SizeOf<T>();
    var random = new Random(seed);

    var buffer = new byte[elementSize];
    for(var i = 0; i < elementCount; i++)
    {
      random.NextBytes(buffer);
      yield return Unsafe.ReadUnaligned<T>(ref buffer[0]);
    }
  }

  #endregion

  #endregion

}