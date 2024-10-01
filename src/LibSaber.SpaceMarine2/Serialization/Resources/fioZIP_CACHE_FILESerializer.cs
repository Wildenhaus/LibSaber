using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Structures.Resources;

namespace LibSaber.SpaceMarine2.Serialization.Resources;

public class fioZIP_CACHE_FILESerializer : SM2SerializerBase<fioZIP_CACHE_FILE>
{

  public override fioZIP_CACHE_FILE Deserialize(NativeReader reader, ISerializationContext context)
  {
    var cache = new fioZIP_CACHE_FILE();

    Span<byte> versionBuffer = stackalloc byte[5];
    Span<byte> entryBuffer = stackalloc byte[26];

    while (reader.Position < reader.Length)
    {
      // 1st Read: Version and Unk (5 bytes)
      reader.Read(versionBuffer);
      var version = BitConverter.ToInt32(versionBuffer.Slice(0,4));
      var unk_04 = versionBuffer[4];

      // 2nd Read: FileName (Variable bytes)
      var fileName = reader.ReadLengthPrefixedString32();

      // 3rd Read: Entry Info (26 bytes)
      reader.Read(entryBuffer);
      var offset = BitConverter.ToInt64(entryBuffer.Slice(0,8));
      var size = BitConverter.ToInt64(entryBuffer.Slice(8,8));
      var compressedSize = BitConverter.ToInt64(entryBuffer.Slice(16,8));
      var compressMethod = (fioZIP_CACHE_FILE.COMPRESS_METHOD)BitConverter.ToInt16(entryBuffer.Slice(24,2));

      var entry = new fioZIP_CACHE_FILE.ENTRY
      {
        FileName = fileName,
        Offset = offset,
        Size = size,
        CompressedSize = compressedSize,
        CompressMethod = compressMethod
      };

      // 4th Read: Version 6+ data
      if (version > 5)
      {
        _ = reader.ReadInt32(); // CRC?
      }

      cache.AddEntry(entry);
    }

    return cache;
  }

}


