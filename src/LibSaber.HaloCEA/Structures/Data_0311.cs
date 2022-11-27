﻿using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;

namespace LibSaber.HaloCEA.Structures
{

  [Sentinel( SentinelIds.Sentinel_0311 )]
  public struct Data_0311 : ISerialData<Data_0311>
  {

    public short Count;
    public List<LodDefinition> LodDefinitions;

    public static implicit operator List<LodDefinition>( Data_0311 list )
      => list.LodDefinitions;

    public static Data_0311 Deserialize( NativeReader reader, ISerializationContext context )
    {
      var data = new Data_0311();

      var count = data.Count = reader.ReadInt16();

      var list = data.LodDefinitions = new List<LodDefinition>( count );
      for ( var i = 0; i < count; i++ )
        list.Add( LodDefinition.Deserialize( reader, context ) );

      return data;
    }
  }

}
