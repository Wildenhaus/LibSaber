using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;

namespace LibSaber.HaloCEA.Structures
{

  [Sentinel( SentinelIds.Sentinel_010E )]
  public struct Data_010E : ISerialData<Data_010E>
  {

    #region Data Members

    public int Unk_00; // material id?
    public int Unk_01;
    public byte Unk_02;
    public byte Unk_03;

    #endregion

    public static Data_010E Deserialize( NativeReader reader, ISerializationContext context )
    {
      return new Data_010E
      {
        Unk_00 = reader.ReadInt32(),
        Unk_01 = reader.ReadInt32(),
        Unk_02 = reader.ReadByte(),
        Unk_03 = reader.ReadByte(),
      };
    }

  }

}
