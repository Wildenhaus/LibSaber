using LibSaber.IO;
using LibSaber.Serialization;

namespace LibSaber.HaloCEA.Structures
{

  public class Data_021E : TypeWrapper<Data_01BA>, ISerialData<Data_021E>
  {

    public static Data_021E Deserialize( NativeReader reader, ISerializationContext context )
    {
      var data = new Data_021E();

      var sentinelReader = new SentinelReader( reader );

      sentinelReader.Next();
      ASSERT( sentinelReader.SentinelId == SentinelIds.Sentinel_01BA );
      data.Value = Data_01BA.Deserialize( reader, context );

      return data;
    }

  }

}
