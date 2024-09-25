using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class objGEOM_STREAM_TO_VBUFFERSerializer : SM2SerializerBase<List<objGEOM_STREAM_TO_VBUFFER>>
{

  public override List<objGEOM_STREAM_TO_VBUFFER> Deserialize(NativeReader reader, ISerializationContext context)
  {
    var count = reader.ReadInt32();
    var propertyCount = reader.ReadInt32();

    var streams = new List<objGEOM_STREAM_TO_VBUFFER>();
    for (var i = 0; i < count; i++)
      streams.Add(new objGEOM_STREAM_TO_VBUFFER());

    if (propertyCount > 0)
      ReadVBufIdxProperty(reader, streams);
    if(propertyCount > 1)
      ReadVBufOffsetProperty(reader, streams);

    return streams;
  }

  private void ReadVBufIdxProperty(NativeReader reader, List<objGEOM_STREAM_TO_VBUFFER> streams)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var stream in streams)
      stream.vBufIdx = reader.ReadInt32();
  }

  private void ReadVBufOffsetProperty(NativeReader reader, List<objGEOM_STREAM_TO_VBUFFER> streams)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var stream in streams)
      stream.vBufOffset = reader.ReadInt32();
  }


}
