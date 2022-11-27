using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;
using LibSaber.Shared.Data;

namespace LibSaber.HaloCEA.Structures
{

  [Sentinel( SentinelIds.Sentinel_0130 )]
  public struct Data_0130 : ISerialData<Data_0130>
  {

    public int Count;
    public BitSet<short> Flags;

    public byte ElementSize;
    public byte[] ElementData;

    public static Data_0130 Deserialize( NativeReader reader, ISerializationContext context )
    {
      var obj = context.GetMostRecentObject<SaberObject>();
      var data = new Data_0130();

      data.Count = reader.ReadInt32();
      data.Flags = BitSet<short>.Deserialize( reader, context );

      data.ElementSize = reader.ReadByte();

      var byteCount = obj.ObjectInfo.VertexCount * data.ElementSize;
      var buffer = data.ElementData = new byte[ byteCount ];
      reader.Read( buffer );

      return data;
    }
  }

}
