﻿using LibSaber.HaloCEA;
using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;

[Sentinel( SentinelIds.Sentinel_0286 )]
public class Data_0286 : ISerialData<Data_0286>
{

  public int Unk_00;
  public int Unk_01;

  public static Data_0286 Deserialize( NativeReader reader, ISerializationContext context )
  {
    var data = new Data_0286();

    data.Unk_00 = reader.ReadInt32();
    data.Unk_01 = reader.ReadInt32();

    return data;
  }

}