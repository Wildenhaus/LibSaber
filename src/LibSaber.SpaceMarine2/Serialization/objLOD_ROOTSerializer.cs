using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class objLOD_ROOTSerializer : SM2SerializerBase<List<objLOD_ROOT>>
{

  #region Overrides

  public override List<objLOD_ROOT> Deserialize(NativeReader reader, ISerializationContext context)
  {
    var count = reader.ReadUInt32();
    var propertyCount = reader.ReadUInt32();

    var lodRoots = new List<objLOD_ROOT>();
    for (var i = 0; i < count; i++)
      lodRoots.Add(new objLOD_ROOT());

    if (propertyCount > 0)
      ReadObjectIdsProperty(reader, lodRoots);
    if (propertyCount > 1)
      ReadMaxObjectLodIndicesProperty(reader, lodRoots);
    if (propertyCount > 2)
      ReadLodDistancesProperty(reader, lodRoots, context);
    if (propertyCount > 3)
      ReadBoundingBoxProperty(reader, lodRoots);
    if (propertyCount > 4)
      ReadSkipFloatProp(reader, lodRoots);
    if (propertyCount > 5)
      ReadDontApplyBiasProp(reader, lodRoots);

    return lodRoots;
  }

  #endregion

  #region Private Methods

  private void ReadObjectIdsProperty(NativeReader reader, List<objLOD_ROOT> lodRoots)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var lodRoot in lodRoots)
    {
      var count = reader.ReadInt32();
      lodRoot.objIds = new List<uint>(count);
      for (var i = 0; i < count; i++)
        lodRoot.objIds.Add(reader.ReadUInt32());
    }
  }

  private void ReadMaxObjectLodIndicesProperty(NativeReader reader, List<objLOD_ROOT> lodRoots)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var lodRoot in lodRoots)
    {
      var count = reader.ReadInt32();
      lodRoot.maxObjectLogIndices = new List<uint>(count);
      for (var i = 0; i < count; i++)
        lodRoot.maxObjectLogIndices.Add(reader.ReadUInt32());
    }
  }

  private void ReadLodDistancesProperty(NativeReader reader, List<objLOD_ROOT> lodRoots, ISerializationContext context)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var lodRoot in lodRoots)
      lodRoot.lodDists = Serializer<List<objLOD_DIST>>.Deserialize(reader, context);
  }

  private void ReadBoundingBoxProperty(NativeReader reader, List<objLOD_ROOT> lodRoots)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var lodRoot in lodRoots)
      lodRoot.bbox = new m3dBOX(reader.ReadVector3(), reader.ReadVector3());
  }

  private void ReadSkipFloatProp(NativeReader reader, List<objLOD_ROOT> lodRoots)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var lodRoot in lodRoots)
      lodRoot.SkipFloat = reader.ReadFloat32();
  }

  private void ReadDontApplyBiasProp(NativeReader reader, List<objLOD_ROOT> lodRoots)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var lodRoot in lodRoots)
      lodRoot.dontApplyBias = reader.ReadBoolean();
  }

  #endregion

}
