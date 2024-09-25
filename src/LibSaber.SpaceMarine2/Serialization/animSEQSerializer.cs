using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class animSEQSerializer : SM2SerializerBase<List<animSEQ>>
{

  public override List<animSEQ> Deserialize(NativeReader reader, ISerializationContext context)
  {
    var count = reader.ReadInt32();
    var propertyCount = reader.ReadInt32();

    var seqList = new List<animSEQ>(count);
    for (var i = 0; i < count; i++)
      seqList.Add(new animSEQ());

    ReadNameProperty(reader, seqList);
    ReadLayerIdProperty(reader, seqList);
    ReadStartFrameProperty(reader, seqList);
    ReadEndFrameProperty(reader, seqList);
    ReadOffsetFrameProperty(reader, seqList);
    ReadLenFrameProperty(reader, seqList);
    ReadTimeSecProperty(reader, seqList);
    ReadActionFramesProperty(reader, seqList);
    ReadBoundingBoxProperty(reader, seqList);

    return seqList;
  }

  #region Private Methods

  private void ReadNameProperty(NativeReader reader, List<animSEQ> seqList)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    for (var i = 0; i < seqList.Count; i++)
      seqList[i].name = reader.ReadLengthPrefixedString32();
  }

  private void ReadLayerIdProperty(NativeReader reader, List<animSEQ> seqList)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    for (var i = 0; i < seqList.Count; i++)
      seqList[i].layerId = reader.ReadUInt32();
  }

  private void ReadStartFrameProperty(NativeReader reader, List<animSEQ> seqList)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    for (var i = 0; i < seqList.Count; i++)
      seqList[i].startFrame = reader.ReadFloat32();
  }

  private void ReadEndFrameProperty(NativeReader reader, List<animSEQ> seqList)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    for (var i = 0; i < seqList.Count; i++)
      seqList[i].endFrame = reader.ReadFloat32();
  }

  private void ReadOffsetFrameProperty(NativeReader reader, List<animSEQ> seqList)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    for (var i = 0; i < seqList.Count; i++)
      seqList[i].offsetFrame = reader.ReadFloat32();
  }

  private void ReadLenFrameProperty(NativeReader reader, List<animSEQ> seqList)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    for (var i = 0; i < seqList.Count; i++)
      seqList[i].lenFrame = reader.ReadFloat32();
  }

  private void ReadTimeSecProperty(NativeReader reader, List<animSEQ> seqList)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    for (var i = 0; i < seqList.Count; i++)
      seqList[i].timeSec = reader.ReadFloat32();
  }

  private void ReadActionFramesProperty(NativeReader reader, List<animSEQ> seqList)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    //var serializer = Serializer.GetSerializer<List<ActionFrame>>();
    //for (var i = 0; i < seqList.Count; i++)
    //  seqList[i].actionFrames = serializer.Deserialize(reader, null);

    foreach(var seq in seqList)
    {
      var framesCount = reader.ReadInt32();
      _ = reader.ReadInt32();
      _ = reader.ReadByte();

      if (framesCount == 0) continue;

      for (var i = 0; i < framesCount; i++)
        _ = reader.ReadInt32();

      if(reader.ReadByte() != 0)
      {
        for (var i = 0; i < framesCount; i++)
          _ = reader.ReadLengthPrefixedString32();
      }
    }
  }

  private void ReadBoundingBoxProperty(NativeReader reader, List<animSEQ> seqList)
  {
    // Read Sentinel
    if (reader.ReadByte() == 0)
      return;

    for (var i = 0; i < seqList.Count; i++)
      seqList[i].bbox = new m3dBOX(reader.ReadVector3(), reader.ReadVector3());
  }

  #endregion

}
