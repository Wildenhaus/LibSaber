using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;

namespace LibSaber.HaloCEA.Structures
{

  [Sentinel( SentinelIds.ObjectSubmeshData )]
  public struct Data_0107 : ISerialData<Data_0107>
  {

    #region Data Members

    [Sentinel( SentinelIds.Sentinel_0104 )] public List<Data_0104_0137> Sentinel_0104;
    [Sentinel( SentinelIds.Sentinel_0109 )] public List<Data_0108> Sentinel_0109;
    [Sentinel( SentinelIds.Sentinel_0137 )] public List<Data_0104_0137> Sentinel_0137;

    [Sentinel( 0x00F3 )]
    public List<Data_0107_00F3> Sentinel_00F3;

    #endregion

    #region Serialization

    public static Data_0107 Deserialize( NativeReader reader, ISerializationContext context )
    {
      var data = new Data_0107();

      var sentinelReader = new SentinelReader( reader );
      while ( sentinelReader.Next() )
      {
        switch ( sentinelReader.SentinelId )
        {
          case SentinelIds.Sentinel_0104:
            data.Sentinel_0104 = DataList<Data_0104_0137>.Deserialize( reader, context );
            break;
          case SentinelIds.Sentinel_0109:
            data.Sentinel_0109 = DataList<Data_0108>.Deserialize( reader, context );
            break;
          case SentinelIds.Sentinel_0137:
            data.Sentinel_0137 = DataList<Data_0104_0137>.Deserialize( reader, context );
            break;

          case 0x00F3:
            data.Sentinel_00F3 = DataList<Data_0107_00F3>.Deserialize( reader, context );
            break;


          case SentinelIds.Delimiter:
            return data;

          default:
            sentinelReader.ReportUnknownSentinel();
            break;
        }
      }

      return data;
    }

    #endregion

  }

}
