using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class tplLOD_DEFSerializer : SM2SerializerBase<List<tplLOD_DEF>>
{

  #region Overrides

  public override List<tplLOD_DEF> Deserialize(NativeReader reader, ISerializationContext context)
  {
    var count = reader.ReadInt32();
    var propertyCount = reader.ReadInt32();

    var lodDefs = new List<tplLOD_DEF>(count);
    for (var i = 0; i < count; i++)
      lodDefs.Add(new tplLOD_DEF());

    ReadObjectIdProperty(reader, lodDefs);
    ReadIndexProperty(reader, lodDefs);
    ReadIsLastLodProperty(reader, lodDefs);

    return lodDefs;
  }

  #endregion

  #region Private Methods

  private void ReadObjectIdProperty(NativeReader reader, List<tplLOD_DEF> lodDefs)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var lodDef in lodDefs)
      lodDef.objId = reader.ReadInt16();
  }

  private void ReadIndexProperty(NativeReader reader, List<tplLOD_DEF> lodDefs)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var lodDef in lodDefs)
      lodDef.index = reader.ReadByte();
  }

  private void ReadIsLastLodProperty(NativeReader reader, List<tplLOD_DEF> lodDefs)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var lodDef in lodDefs)
      lodDef.isLastLodUpToInfinity = reader.ReadBoolean();
  }

  #endregion

}
