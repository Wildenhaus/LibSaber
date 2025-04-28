namespace LibSaber.IO.Tests;

public abstract class NativeReaderTestsBase
{

  protected Endianness HostEndianness =>
    BitConverter.IsLittleEndian
      ? Endianness.LittleEndian
      : Endianness.BigEndian;

  protected Endianness ReverseEndianness =>
    BitConverter.IsLittleEndian
      ? Endianness.BigEndian
      : Endianness.LittleEndian;

}
