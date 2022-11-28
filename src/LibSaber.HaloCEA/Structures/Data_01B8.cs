using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;

namespace LibSaber.HaloCEA.Structures
{

  [Sentinel( SentinelIds.Sentinel_01B8 )]
  public class Data_01B8 : List<Data_01B8_Entry>, ISerialData<Data_01B8>
  {

    public Data_01B8()
    {
    }

    public Data_01B8( int capacity )
      : base( capacity )
    {
    }

    public static Data_01B8 Deserialize( NativeReader reader, ISerializationContext context )
    {
      var count = reader.ReadInt32();
      var data = new Data_01B8( count );

      var sentinelReader = new SentinelReader( reader );
      for ( var i = 0; i < count; i++ )
      {
        data.Add( Data_01B8_Entry.Deserialize( reader, context ) );
      }

      return data;
    }

  }

  public class Data_01B8_Entry : ISerialData<Data_01B8_Entry>
  {

    public Data_01B9 Data_01B9;
    public string Data_01BB;

    public static Data_01B8_Entry Deserialize( NativeReader reader, ISerializationContext context )
    {
      var data = new Data_01B8_Entry();

      var sentinelReader = new SentinelReader( reader );

      sentinelReader.Next();
      ASSERT( sentinelReader.SentinelId == SentinelIds.Sentinel_01B9 );
      data.Data_01B9 = Data_01B9.Deserialize( reader, context );

      sentinelReader.Next();
      ASSERT( sentinelReader.SentinelId == SentinelIds.Sentinel_01BB );
      data.Data_01BB = reader.ReadNullTerminatedString();

      return data;
    }

  }

}
