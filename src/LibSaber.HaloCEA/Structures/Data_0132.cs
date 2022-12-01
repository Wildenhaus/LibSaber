﻿using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;

namespace LibSaber.HaloCEA.Structures
{

  [Sentinel( SentinelIds.Sentinel_0132 )]
  public struct Data_0132 : ISerialData<Data_0132>
  {

    #region Data Members

    public short FirstDependentObjectId;
    public byte DependentObjectCount;
    public short UnkObjId02;
    public byte Unk_03;

    #endregion

    #region Serialization

    public static Data_0132 Deserialize( NativeReader reader, ISerializationContext context )
    {
      var data = new Data_0132();

      /*  FirstDependentObjectId is the Id of the first object that will use this
       *  object's geometry data. 
       *  DependentObjectCount is the number of objects that also use this object's
       *  data. The other dependent object Ids are sequential, so if the count is 5,
       *  the next 5 Ids will also be dependent objects.
       */
      data.FirstDependentObjectId = reader.ReadInt16();
      data.DependentObjectCount = reader.ReadByte();

      // Guessing this is dependencies?
      data.UnkObjId02 = reader.ReadInt16();
      data.Unk_03 = reader.ReadByte();

      return data;
    }

    #endregion

  }

}
