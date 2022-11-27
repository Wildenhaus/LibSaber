using LibSaber.IO;
using LibSaber.Serialization;

namespace LibSaber.HaloCEA.Structures
{

  public struct ActionFrame : ISerialData<ActionFrame>
  {

    public byte Unk_00;
    public int Frame;
    public string Comment;

    public static ActionFrame Deserialize( NativeReader reader, ISerializationContext context )
    {
      return new ActionFrame
      {
        Unk_00 = reader.ReadByte(),
        Frame = reader.ReadInt32(),
        Comment = reader.ReadPascalString32(),
      };
    }
  }

}
