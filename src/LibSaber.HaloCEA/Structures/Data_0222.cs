using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;

namespace LibSaber.HaloCEA.Structures
{

  [Sentinel( SentinelIds.Sentinel_0222 )]
  public class Data_0222 : ISerialData<Data_0222>
  {

    #region Data Members

    public int Unk_00;
    public int Unk_01;
    public byte[] Unk_02;
    public byte[] Unk_03;
    public byte[] Unk_04;

    #endregion

    #region Serialization

    public static Data_0222 Deserialize( NativeReader reader, ISerializationContext context )
    {
      var data = new Data_0222();

      data.Unk_00 = reader.ReadInt32();
      data.Unk_01 = reader.ReadInt32();

      var parentData = context.GetMostRecentObject<Data_0221>();

      var unk_02_size = ( parentData.Unk_01 + 1 ) * 4;
      data.Unk_02 = new byte[ unk_02_size ];
      reader.Read( data.Unk_02 );

      var unk_03_size = data.Unk_01 * 2;
      data.Unk_03 = new byte[ unk_03_size ];
      reader.Read( data.Unk_03 );

      var unk_04_size = ( parentData.Unk_00 + 1 ) * 4;
      data.Unk_04 = new byte[ unk_04_size ];
      reader.Read( data.Unk_04 );

      return data;
    }

    #endregion

  }

}
