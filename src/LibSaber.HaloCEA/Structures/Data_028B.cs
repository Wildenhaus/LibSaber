using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;

namespace LibSaber.HaloCEA.Structures
{

  [Sentinel( SentinelIds.Sentinel_028B )]
  public class Data_028B : ISerialData<Data_028B>
  {

    #region Data Members

    public int Unk_00;
    public short Unk_01;
    public byte Unk_02;

    #endregion

    #region Serialization

    public static Data_028B Deserialize( NativeReader reader, ISerializationContext context )
    {
      var data = new Data_028B();

      data.Unk_00 = reader.ReadInt32();
      data.Unk_01 = reader.ReadInt16();
      data.Unk_02 = reader.ReadByte();

      return data;
    }

    #endregion

  }

}
