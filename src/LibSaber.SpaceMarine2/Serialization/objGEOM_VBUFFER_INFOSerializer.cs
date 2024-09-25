using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class objGEOM_VBUFFER_INFOSerializer : SM2SerializerBase<List<objGEOM_VBUFFER_INFO>>
{

  public override List<objGEOM_VBUFFER_INFO> Deserialize(NativeReader reader, ISerializationContext context)
  {
    var count = reader.ReadInt32();
    var propertyCount = reader.ReadInt32();

    var infos = new List<objGEOM_VBUFFER_INFO>();
    for (var i = 0; i < count; i++)
      infos.Add(new objGEOM_VBUFFER_INFO());

    if (propertyCount > 0)
      ReadSizeProperty(reader, infos);
    if (propertyCount > 1)
      ReadSkipIntProperty(reader, infos);
    if (propertyCount > 2)
      ReadOldFlagsProperty(reader, infos);
    if( propertyCount > 3)
      ReadNewFlagsProperty(reader, infos);

    return infos;
  }

  private void ReadSizeProperty(NativeReader reader, List<objGEOM_VBUFFER_INFO> infos)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach(var info in infos)
      info.size = reader.ReadInt32();
  }

  private void ReadSkipIntProperty(NativeReader reader, List<objGEOM_VBUFFER_INFO> infos)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var info in infos)
      info.skip_int = reader.ReadInt32();
  }

  private void ReadOldFlagsProperty(NativeReader reader, List<objGEOM_VBUFFER_INFO> infos)
  {
    if (reader.ReadByte() == 0)
      return;

    FAIL("Not Implemented: objGEOM_VBUFFER_INFO old flags");
  }

  private void ReadNewFlagsProperty(NativeReader reader, List<objGEOM_VBUFFER_INFO> infos)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var info in infos)
    {
      if (reader.ReadByte() == 0)
        continue;

      info.geomVbufferFlags = reader.ReadUInt16();
    }
  }

}
