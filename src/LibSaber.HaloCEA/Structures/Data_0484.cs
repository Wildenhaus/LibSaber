﻿using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;

namespace LibSaber.HaloCEA.Structures
{

  [Sentinel( SentinelIds.Sentinel_0484 )]
  public class Data_0484 : List<Data_0484_Entry>, ISerialData<Data_0484>
  {

    #region Constructor

    public Data_0484()
    {
    }

    public Data_0484( int capacity )
      : base( capacity )
    {
    }

    #endregion

    #region Serialization

    public static Data_0484 Deserialize( NativeReader reader, ISerializationContext context )
    {
      var count = reader.ReadInt16();
      var data = new Data_0484( count );

      for ( var i = 0; i < count; i++ )
        data.Add( Data_0484_Entry.Deserialize( reader, context ) );

      return data;
    }

    #endregion

  }

  public class Data_0484_Entry : ISerialData<Data_0484_Entry>
  {

    public string TypeName;
    public string TemplateName;
    public short Unk_Id;


    public static Data_0484_Entry Deserialize( NativeReader reader, ISerializationContext context )
    {
      var data = new Data_0484_Entry();

      data.TypeName = reader.ReadPascalString32();
      data.TemplateName = reader.ReadPascalString32();
      data.Unk_Id = reader.ReadInt16();

      return data;
    }

  }

}