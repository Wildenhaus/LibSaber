using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class animOBJ_ANIMSerializer : SM2SerializerBase<List<animOBJ_ANIM>>
{

  public override List<animOBJ_ANIM> Deserialize(NativeReader reader, ISerializationContext context)
  {
    var count = reader.ReadInt32();
    var propertyCount = reader.ReadUInt32();

    var animList = new List<animOBJ_ANIM>(count);
    for (var i = 0; i < count; i++)
      animList.Add(new animOBJ_ANIM());

    if (propertyCount > 0)
      ReadIniTranslationProperty(reader, animList);
    if (propertyCount > 1)
      ReadPTranslationProperty(reader, animList, context);
    if (propertyCount > 2)
      ReadIniRotationProperty(reader, animList);
    if (propertyCount > 3)
      ReadPRotationProperty(reader, animList, context);
    if (propertyCount > 4)
      ReadIniScaleProperty(reader, animList);
    if (propertyCount > 5)
      ReadPScaleProperty(reader, animList, context);
    if (propertyCount > 6)
      ReadIniVisibilityProperty(reader, animList);
    if (propertyCount > 7)
      ReadPVisibilityProperty(reader, animList, context);

    return animList;
  }

  #region Private Methods

  private void ReadIniTranslationProperty(NativeReader reader, List<animOBJ_ANIM> animList)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    foreach (var anim in animList)
      anim.iniTranslation = reader.ReadVector3();
  }

  private void ReadPTranslationProperty(NativeReader reader, List<animOBJ_ANIM> animList, ISerializationContext context)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    var propertyFlags = reader.ReadBitArray(animList.Count);

    for (var i = 0; i < animList.Count; i++)
      if (propertyFlags[i])
        animList[i].pTranslation = Serializer<m3dSPL>.Deserialize(reader, context);
  }

  private void ReadIniRotationProperty(NativeReader reader, List<animOBJ_ANIM> animList)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    foreach (var anim in animList)
      anim.iniRotation = reader.ReadVector4();
  }

  private void ReadPRotationProperty(NativeReader reader, List<animOBJ_ANIM> animList, ISerializationContext context)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    var propertyFlags = reader.ReadBitArray(animList.Count);

    for (var i = 0; i < animList.Count; i++)
      if (propertyFlags[i])
        animList[i].pRotation = Serializer<m3dSPL>.Deserialize(reader, context);
  }

  private void ReadIniScaleProperty(NativeReader reader, List<animOBJ_ANIM> animList)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    foreach (var anim in animList)
      anim.iniScale = reader.ReadVector3();
  }

  private void ReadPScaleProperty(NativeReader reader, List<animOBJ_ANIM> animList, ISerializationContext context)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    var propertyFlags = reader.ReadBitArray(animList.Count);

    for (var i = 0; i < animList.Count; i++)
      if (propertyFlags[i])
        animList[i].pScale = Serializer<m3dSPL>.Deserialize(reader, context);
  }

  private void ReadIniVisibilityProperty(NativeReader reader, List<animOBJ_ANIM> animList)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    foreach (var anim in animList)
      anim.iniVisibility = reader.ReadFloat32();
  }

  private void ReadPVisibilityProperty(NativeReader reader, List<animOBJ_ANIM> animList, ISerializationContext context)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    var propertyFlags = reader.ReadBitArray(animList.Count);

    for (var i = 0; i < animList.Count; i++)
      if (propertyFlags[i])
        animList[i].pVisibility = Serializer<m3dSPL>.Deserialize(reader, context);
  }

  #endregion


}
