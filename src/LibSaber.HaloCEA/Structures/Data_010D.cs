using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;

namespace LibSaber.HaloCEA.Structures
{

  [Sentinel( SentinelIds.Sentinel_010D )]
  public struct Data_010D : ISerialData<Data_010D>
  {

    public int VertexOffset;
    public int VertexCount;

    public static Data_010D Deserialize( NativeReader reader, ISerializationContext context )
    {
      return new Data_010D
      {
        VertexOffset = reader.ReadInt32(),
        VertexCount = reader.ReadInt32(),
      };
    }
  }

}
