using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class objLOD_DISTSerializer : SM2SerializerBase<List<objLOD_DIST>>
{


  #region Overrides

  public override List<objLOD_DIST> Deserialize(NativeReader reader, ISerializationContext context)
  {
    var count = reader.ReadInt32();
    var propertyCount = reader.ReadUInt32();

    var lodDists = new List<objLOD_DIST>(count);
    for (var i = 0; i < count; i++)
      lodDists.Add(new objLOD_DIST());

    ReadMaxDistanceProperty(reader, lodDists);

    return lodDists;
  }

  #endregion

  #region Private Methods

  private void ReadMaxDistanceProperty(NativeReader reader, List<objLOD_DIST> lodDists)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var lodDist in lodDists)
      lodDist.maxDist = reader.ReadFloat32();
  }

  #endregion


}
