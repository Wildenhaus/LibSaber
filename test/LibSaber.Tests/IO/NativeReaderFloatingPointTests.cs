namespace LibSaber.IO.Tests;

public class NativeReaderFloatingPointTests : NativeReaderTestsBase
{

  [Fact]
  public void NativeReader_reads_float_endian_swap()
  {
    var random = new Random();
    var expectedVal = (float)random.NextDouble();
    var bytes = BitConverter.GetBytes(expectedVal).Reverse().ToArray();
    var stream = new MemoryStream(bytes);

    var reader = new NativeReader(stream, ReverseEndianness);

    var readValue = reader.ReadFloat32();
    Assert.Equal(expectedVal, readValue);
  }

  [Fact]
  public void NativeReader_reads_double_endian_swap()
  {
    var random = new Random();
    var expectedVal = random.NextDouble();
    var bytes = BitConverter.GetBytes(expectedVal).Reverse().ToArray();
    var stream = new MemoryStream(bytes);

    var reader = new NativeReader(stream, ReverseEndianness);

    var readValue = reader.ReadFloat64();
    Assert.Equal(expectedVal, readValue);
  }

}
