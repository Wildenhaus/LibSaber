using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class objOBJSerializer : SM2SerializerBase<List<objOBJ>>
{

  public override List<objOBJ> Deserialize(NativeReader reader, ISerializationContext context)
  {
    var graph = context.GetMostRecentObject<objGEOM_MNG>();
    var objectCount = reader.ReadInt16();

    var objects = new List<objOBJ>(objectCount);
    for (var i = 0; i < objectCount; i++)
      objects.Add(new objOBJ(graph));

    context.AddObject(objects);

    var unk_01 = reader.ReadUInt16();
    var propertyCount = reader.ReadUInt16();
    var unk_03 = reader.ReadUInt16();

    
    if (propertyCount > 0)
      ReadIdProperty(reader, objects);
    if (propertyCount > 1)
      ReadReadNameProperty(reader, objects);
    if (propertyCount > 2)
      ReadStateProperty(reader, objects);
    if (propertyCount > 3)
      ReadParentIdProperty(reader, objects);
    if (propertyCount > 4)
      ReadNextIdProperty(reader, objects);
    if (propertyCount > 5)
      ReadPrevIdProperty(reader, objects);
    if (propertyCount > 6)
      ReadChildIdProperty(reader, objects);
    if (propertyCount > 7)
      ReadAnimNumberProperty(reader, objects);
    if (propertyCount > 8)
      ReadReadAffixesProperty(reader, objects);
    if (propertyCount > 9)
      ReadMatrixLTProperty(reader, objects);
    if (propertyCount > 10)
      ReadMatrixModelProperty(reader, objects);
    if (propertyCount > 11)
      ReadGeomDataProperty(reader, objects, context);
    if (propertyCount > 12)
      ReadSourceIdProperty(reader, objects);
    if (propertyCount > 13)
      ReadObbProperty(reader, objects);
    if (propertyCount > 14)
      ReadReadOnlyNameProperty(reader, objects);
    if (propertyCount > 15)
      ReadReadOnlyAffixesProperty(reader, objects);
    if (propertyCount > 16)
      ReadReadOnlyObjectNameProperty(reader, objects);
    if (propertyCount > 17)
      ReadAffixesProperty(reader, objects);

    return objects;
  }

  #region Private Methods

  private void ReadIdProperty(NativeReader reader, List<objOBJ> objects)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    foreach (var obj in objects)
      obj.id = reader.ReadInt16();
  }

  private void ReadReadNameProperty(NativeReader reader, List<objOBJ> objects)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    foreach (var obj in objects)
      obj.ReadName = reader.ReadLengthPrefixedString32();
  }

  private void ReadStateProperty(NativeReader reader, List<objOBJ> objects)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    for (var i = 0; i < objects.Count; i++)
    {
      _ = reader.ReadUInt32(); // TODO: Unk
      _ = reader.ReadUInt32(); // TODO: Unk
    }
  }

  private void ReadParentIdProperty(NativeReader reader, List<objOBJ> objects)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    foreach (var obj in objects)
      obj.parentId = reader.ReadInt16();
  }

  private void ReadNextIdProperty(NativeReader reader, List<objOBJ> objects)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    foreach (var obj in objects)
      obj.nextId = reader.ReadInt16();
  }

  private void ReadPrevIdProperty(NativeReader reader, List<objOBJ> objects)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    foreach (var obj in objects)
      obj.prevId = reader.ReadInt16();
  }

  private void ReadChildIdProperty(NativeReader reader, List<objOBJ> objects)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    foreach (var obj in objects)
      obj.childId = reader.ReadInt16();
  }

  private void ReadAnimNumberProperty(NativeReader reader, List<objOBJ> objects)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    foreach (var obj in objects)
      obj.animNmb = reader.ReadInt16();
  }

  private void ReadReadAffixesProperty(NativeReader reader, List<objOBJ> objects)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    foreach (var obj in objects)
      obj.ReadAffixes = reader.ReadLengthPrefixedString32();
  }

  private void ReadMatrixLTProperty(NativeReader reader, List<objOBJ> objects)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    foreach (var obj in objects)
      obj.MatrixLT = reader.ReadMatrix4x4();
  }

  private void ReadMatrixModelProperty(NativeReader reader, List<objOBJ> objects)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    foreach (var obj in objects)
      obj.MatrixModel = reader.ReadMatrix4x4();
  }

  private void ReadGeomDataProperty(NativeReader reader, List<objOBJ> objects, ISerializationContext context)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    var unshared = Serializer<List<objGEOM_UNSHARED>>.Deserialize(reader, context);
    for (var i = 0; i < objects.Count; i++)
      objects[i].geomData = unshared[i];

    //var objFlags = reader.ReadBitArray(objects.Count);

    //for (var i = 0; i < objects.Count; i++)
    //{
    //  if (objFlags[i])
    //  {
    //    var unk_01 = reader.ReadUInt32(); // TODO: 0x00000003
    //    var unk_02 = reader.ReadByte(); // TODO: 0x7
    //    ASSERT(unk_01 == 0x4, "4 val not found");
    //    //ASSERT(unk_02 == 0xC, "C val not found");

    //    var geomData = new objGEOM_UNSHARED();
    //    geomData.splitIndex = reader.ReadUInt32();
    //    geomData.numSplits = reader.ReadUInt32();
    //    geomData.bbox = new m3dBOX(reader.ReadVector3(), reader.ReadVector3());
    //    geomData.obb = new m3dBOX(reader.ReadVector3(), reader.ReadVector3());
    //    reader.ReadVector3();
    //    reader.ReadVector3();
    //    reader.ReadByte();
    //    reader.ReadByte();
    //    reader.ReadByte();
    //    reader.ReadByte();

    //    objects[i].geomData = geomData;
    //  }
    //}

    //ASSERT(reader.PeekByte() != 0x3, "Still more objGEOM_UNSHARED data");
    //    //reader.ReadByte();
    //    //reader.ReadByte();
    //    //reader.ReadByte();
    //    //reader.ReadByte();
    //    //reader.ReadByte();
  }

  private void ReadUnkNamesProperty(NativeReader reader, List<objOBJ> objects)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    foreach (var obj in objects)
      obj.UnkName = reader.ReadLengthPrefixedString32();
  }

  private void ReadSourceIdProperty(NativeReader reader, List<objOBJ> objects)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach(var obj in objects)
      obj.SourceId = reader.ReadLengthPrefixedString32();
  }

  private void ReadObbProperty(NativeReader reader, List<objOBJ> objects)
  {
    // TODO: Move this into M3DOBB serializer/data class
    // This seems to be all zeroes.
    // Maybe this is the skip[dsString] property?

    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    foreach (var obj in objects)
      for (var j = 0; j < 60; j++)
        reader.ReadByte();
  }

  private void ReadReadOnlyNameProperty(NativeReader reader, List<objOBJ> objects)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var obj in objects)
      obj.ReadOnlyName = reader.ReadLengthPrefixedString16();
  }

  private void ReadReadOnlyAffixesProperty(NativeReader reader, List<objOBJ> objects)
  {
    if (reader.ReadByte() == 0)
      return;

    throw new NotImplementedException();
  }

  private void ReadReadOnlyObjectNameProperty(NativeReader reader, List<objOBJ> objects)
  {
    // Read Sentinel 1d1fd
    if (reader.ReadByte() == 0)
      return;

    foreach(var obj in objects)
      obj.ReadOnlyObjectName = reader.ReadLengthPrefixedString32();
  }

  private void ReadAffixesProperty(NativeReader reader, List<objOBJ> objects)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    foreach (var obj in objects)
      obj.Affixes = reader.ReadLengthPrefixedString32();
  }

  #endregion

}
