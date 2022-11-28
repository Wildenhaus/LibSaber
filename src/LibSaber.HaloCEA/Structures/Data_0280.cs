using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;
using LibSaber.Shared.Structures;

namespace LibSaber.HaloCEA.Structures
{

  [Sentinel( SentinelIds.Sentinel_0280 )]
  public class Data_0280 : List<Data_0280_Entry>, ISerialData<Data_0280>
  {

    public Data_0280()
    {
    }

    public Data_0280( int capacity )
      : base( capacity )
    {
    }

    public static Data_0280 Deserialize( NativeReader reader, ISerializationContext context )
    {
      var count = reader.ReadInt32();

      var data = new Data_0280( count );

      for ( var i = 0; i < count; i++ )
      {
        data.Add( Data_0280_Entry.Deserialize( reader, context ) );
      }

      return data;
    }

  }

  public class Data_0280_Entry : ISerialData<Data_0280_Entry>
  {

    [Sentinel( SentinelIds.Sentinel_028B )]
    public Data_028B Data_028B;

    [Sentinel( SentinelIds.Sentinel_0282 )]
    public string Data_0282;

    [Sentinel( SentinelIds.Sentinel_0283 )]
    public Matrix4<float> Data_0283;

    [Sentinel( SentinelIds.Sentinel_0284 )]
    public Data_0284 Data_0284;

    [Sentinel( SentinelIds.Sentinel_0285 )]
    public Data_0285 Data_0285;

    [Sentinel( SentinelIds.Sentinel_0286 )]
    public Data_0286 Data_0286;

    [Sentinel( SentinelIds.Sentinel_0289 )]
    public string Data_0289;

    [Sentinel( SentinelIds.Sentinel_028A )]
    public string Data_028A;

    [Sentinel( SentinelIds.Sentinel_028C )]
    public Data_028C Data_028C;

    [Sentinel( SentinelIds.Sentinel_0348 )]
    public Data_0348 Data_0348;

    [Sentinel( SentinelIds.Sentinel_0349 )]
    public int Data_0349;

    public static Data_0280_Entry Deserialize( NativeReader reader, ISerializationContext context )
    {
      var data = new Data_0280_Entry();

      var sentinelReader = new SentinelReader( reader );
      while ( sentinelReader.Next() )
      {
        switch ( sentinelReader.SentinelId )
        {
          case SentinelIds.Sentinel_028B:
            data.Data_028B = Data_028B.Deserialize( reader, context );
            break;
          case SentinelIds.Sentinel_0282:
            data.Data_0282 = reader.ReadNullTerminatedString();
            break;
          case SentinelIds.Sentinel_0283:
            data.Data_0283 = Matrix4<float>.Deserialize( reader, context );
            break;
          case SentinelIds.Sentinel_0284:
            data.Data_0284 = Data_0284.Deserialize( reader, context );
            break;
          case SentinelIds.Sentinel_0285:
            data.Data_0285 = Data_0285.Deserialize( reader, context );
            break;
          case SentinelIds.Sentinel_0286:
            data.Data_0286 = Data_0286.Deserialize( reader, context );
            break;
          case SentinelIds.Sentinel_0289:
            data.Data_0289 = reader.ReadNullTerminatedString();
            break;
          case SentinelIds.Sentinel_028A:
            data.Data_028A = reader.ReadNullTerminatedString();
            break;
          case SentinelIds.Sentinel_028C:
            data.Data_028C = Data_028C.Deserialize( reader, context );
            break;
          case SentinelIds.Sentinel_0348:
            data.Data_0348 = Data_0348.Deserialize( reader, context );
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

  }

}
