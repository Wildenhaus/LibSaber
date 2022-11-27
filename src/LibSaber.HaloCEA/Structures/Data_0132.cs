using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;

namespace LibSaber.HaloCEA.Structures
{

  [Sentinel( SentinelIds.Sentinel_0132 )]
  public struct Data_0132 : ISerialData<Data_0132>
  {

    public short UnkObjId01;
    public byte Unk_01;
    public short UnkObjId02;
    public byte Unk_03;

    public static Data_0132 Deserialize( NativeReader reader, ISerializationContext context )
    {
      var data = new Data_0132();

      data.UnkObjId01 = reader.ReadInt16();
      data.Unk_01 = reader.ReadByte();
      data.UnkObjId02 = reader.ReadInt16();
      data.Unk_03 = reader.ReadByte();

      return data;
    }
  }

}
