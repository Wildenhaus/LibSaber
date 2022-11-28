﻿using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Structures;

namespace LibSaber.HaloCEA.Structures
{

  public class Data_0220 : ISerialData<Data_0220>
  {

    public Vector3<int> Unk_00;
    public Vector3<float> Unk_01;
    public Vector3<float> Unk_02;
    public Vector3<float> Unk_03;

    public static Data_0220 Deserialize( NativeReader reader, ISerializationContext context )
    {
      var data = new Data_0220();

      data.Unk_00 = Vector3<int>.Deserialize( reader, context );
      data.Unk_01 = Vector3<float>.Deserialize( reader, context );
      data.Unk_02 = Vector3<float>.Deserialize( reader, context );
      data.Unk_03 = Vector3<float>.Deserialize( reader, context );

      context.AddObject( data );
      return data;
    }

  }

}
