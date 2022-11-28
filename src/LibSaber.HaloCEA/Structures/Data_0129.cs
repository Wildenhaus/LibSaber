using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;

namespace LibSaber.HaloCEA.Structures
{


  [Sentinel( SentinelIds.Sentinel_0129 )]
  public class Data_0129 : ISerialData<Data_0129>
  {

    public short SharedObjectId;
    public int VertexOffset;
    public int FaceOffset;

    public static Data_0129 Deserialize( NativeReader reader, ISerializationContext context )
    {
      var data = new Data_0129();

      data.SharedObjectId = reader.ReadInt16();
      data.VertexOffset = reader.ReadInt32();
      data.FaceOffset = reader.ReadInt32();

      return data;
    }

  }

}
