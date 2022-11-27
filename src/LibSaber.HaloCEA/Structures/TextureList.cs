﻿using LibSaber.IO;
using LibSaber.Serialization;

namespace LibSaber.HaloCEA.Structures
{

  public class TextureList : List<TextureListEntry>, ISerialData<TextureList>
  {

    #region Serialization

    public static TextureList Deserialize( NativeReader reader, ISerializationContext context )
    {
      var data = new TextureList();

      var count = reader.ReadInt32();
      var sentinelReader = new SentinelReader( reader );
      for ( var i = 0; i < count; i++ )
      {
        ASSERT( sentinelReader.Next(), "Unexpected end of TextureList." );
        ASSERT( sentinelReader.SentinelId == SentinelIds.TextureListEntry,
          "Unexpected SentinelId in TextureList: 0x{0:X2}",
          sentinelReader.SentinelId );

        data.Add( TextureListEntry.Deserialize( reader, context ) );
      }

      return data;
    }

    #endregion

  }

}