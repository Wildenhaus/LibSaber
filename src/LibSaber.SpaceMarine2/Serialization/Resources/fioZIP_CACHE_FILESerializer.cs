using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Structures.Resources;

namespace LibSaber.SpaceMarine2.Serialization.Resources;

public class fioZIP_CACHE_FILESerializer : SM2SerializerBase<fioZIP_CACHE_FILE>
{

  public override fioZIP_CACHE_FILE Deserialize(NativeReader reader, ISerializationContext context)
  {
    var cache = new fioZIP_CACHE_FILE();

    while (reader.Position < reader.Length)
    {
      ASSERT(reader.ReadInt32() == 5, "fioZIP_CACHE_FILE::Entry did not start with a 5");
      ASSERT(reader.ReadByte() == 0x1F, "fioZIP_CACHE_FILE::Entry did not start with a 1F");
      var entry = new fioZIP_CACHE_FILE.ENTRY
      {
        FileName = reader.ReadLengthPrefixedString32(),
        Offset = reader.ReadInt64(),
        Size = reader.ReadInt64(),
        CompressedSize = reader.ReadInt64(),
        CompressMethod = (fioZIP_CACHE_FILE.COMPRESS_METHOD)reader.ReadInt16(),
      };

      cache.AddEntry(entry);
    }

    return cache;
  }

}


