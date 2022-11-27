using LibSaber.IO;
using LibSaber.Serialization;

namespace LibSaber.HaloCEA.Structures
{

  public struct Data_012F : ISerialData<Data_012F>
  {

    #region Data Members

    public byte Count;
    public List<CEASentinel012FData> Entries;

    #endregion

    public static Data_012F Deserialize( NativeReader reader, ISerializationContext context )
    {
      var data = new Data_012F();

      var count = data.Count = data.Count = reader.ReadByte();
      var entries = data.Entries = new List<CEASentinel012FData>( count );
      for ( var i = 0; i < count; i++ )
        entries.Add( CEASentinel012FData.Deserialize( reader, context ) );

      return data;
    }

  }

  public struct CEASentinel012FData : ISerialData<CEASentinel012FData>
  {
    public byte Unk_00;
    public int Unk_01;

    public static CEASentinel012FData Deserialize( NativeReader reader, ISerializationContext context )
    {
      return new CEASentinel012FData
      {
        Unk_00 = reader.ReadByte(),
        Unk_01 = reader.ReadInt32()
      };
    }
  }

}
