using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class ActionFrameSerializer : SM2SerializerBase<List<ActionFrame>>
{

  public override List<ActionFrame> Deserialize(NativeReader reader, ISerializationContext context)
  {
    // TODO: This is never used?
    _ = reader.ReadInt32();
    _ = reader.ReadInt32();

    var count = 0;
    if(reader.ReadByte() != 0)
    {
      count = reader.ReadInt32();
      _ = reader.ReadInt32();
    }

    if (reader.ReadByte() != 0)
    {
      for (var i = 0; i < count; i++)
        _ = reader.ReadInt32();
    }

    if (reader.ReadByte() != 0)
    {
      for (var i = 0; i < count; i++)
        _ = reader.ReadLengthPrefixedString32();
    }

    _ = reader.ReadInt32();
    _ = reader.ReadInt32();
    _ = reader.ReadByte();

    return null;
    //throw new NotImplementedException();
  }

}
