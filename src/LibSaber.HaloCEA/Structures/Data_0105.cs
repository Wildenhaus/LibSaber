using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;

namespace LibSaber.HaloCEA.Structures
{

  [Sentinel( SentinelIds.Sentinel_0105 )]
  public struct Data_0105 : ISerialData<Data_0105>
  {

    public int FaceOffset;
    public int FaceCount;

    public static Data_0105 Deserialize( NativeReader reader, ISerializationContext context )
    {
      return new Data_0105
      {
        FaceOffset = reader.ReadInt32(),
        FaceCount = reader.ReadInt32(),
      };
    }
  }

}
