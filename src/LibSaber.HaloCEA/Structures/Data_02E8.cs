﻿using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;

namespace LibSaber.HaloCEA.Structures
{

  [Sentinel( 0x02E8 )]
  public struct Data_02E8 : ISerialData<Data_02E8>
  {

    #region Data Members

    public int Count;
    public List<CEAObjectAnimation> Entries;

    #endregion

    #region Casts

    public static implicit operator List<CEAObjectAnimation>( Data_02E8 dataList )
      => dataList.Entries;

    #endregion

    #region Serialization

    public static Data_02E8 Deserialize( NativeReader reader, ISerializationContext context )
    {
      var data = new Data_02E8();

      var count = data.Count = reader.ReadInt32();

      var sentinelReader = new SentinelReader( reader );
      var entries = data.Entries = new List<CEAObjectAnimation>( count );
      for ( var i = 0; i < count; i++ )
      {
        sentinelReader.Next();
        entries.Add( CEAObjectAnimation.Deserialize( reader, context ) );
      }

      return data;
    }

    #endregion

  }
}
