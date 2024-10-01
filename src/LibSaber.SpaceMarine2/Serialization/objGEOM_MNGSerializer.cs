using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Structures;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class objGEOM_MNGSerializer : SM2SerializerBase<objGEOM_MNG>
{

  #region Constants

  private const uint SIGNATURE_OGM1 = 0x314D474F; //OGM1

  #endregion

  #region Overrides

  public override objGEOM_MNG Deserialize(NativeReader reader, ISerializationContext context)
  {
    ReadSignature(reader, SIGNATURE_OGM1);

    var graph = new objGEOM_MNG();
    context.AddObject(graph);

    var unk_00 = reader.ReadUInt32();
    var varsCount = unk_00 & 0xFFFF; // first 16 bytes are prop count
    var hasBeforeLoadCallback = (unk_00 & 0x20000) != 0;
    var version = reader.ReadUInt16(); // TODO

    objGEOM_MNGFlags propertyFlags = default;
    if (varsCount <= 8)
      propertyFlags = (objGEOM_MNGFlags)reader.ReadByte();
    else
      propertyFlags = (objGEOM_MNGFlags)reader.ReadInt16();

    if (propertyFlags.HasFlag(objGEOM_MNGFlags.Objects))
      ReadObjectsProperty(reader, graph, context);
    if (propertyFlags.HasFlag(objGEOM_MNGFlags.ObjectProps))
      ReadObjectPropsProperty(reader, graph, context);
    if (propertyFlags.HasFlag(objGEOM_MNGFlags.ObjectPS))
      ReadObjectPsProperty(reader, graph, context);
    if (propertyFlags.HasFlag(objGEOM_MNGFlags.LodRoots))
      ReadLodRootsProperty(reader, graph, context);
    if (propertyFlags.HasFlag(objGEOM_MNGFlags.VBufferMapping))
      ReadVBufferMappingProperty(reader, graph, context);
    if (propertyFlags.HasFlag(objGEOM_MNGFlags.NamedObjectNames))
      ReadNamedObjectNames(reader, graph, context);
    if (propertyFlags.HasFlag(objGEOM_MNGFlags.NamedObjectIds))
      ReadNamedObjectIds(reader, graph, context);
    if (propertyFlags.HasFlag(objGEOM_MNGFlags.MatrixLT))
      ReadMatrixLT(reader, graph, context);
    if (propertyFlags.HasFlag(objGEOM_MNGFlags.MatrixModel))
      ReadMatrixModel(reader, graph, context);
    if (propertyFlags.HasFlag(objGEOM_MNGFlags.SplitInfo))
      ReadObjectSplitInfo(reader, graph, context);

    ReadData(reader, graph, context);

    if(graph.vBufferMapping is not null)
    {
      var streamToVbuffer = graph.vBufferMapping.streamToVBuffer;
      var vBufferInfo = graph.vBufferMapping.vBufferInfo;

      
      for (var i = 0; i < graph.BufferCount; i++)
      {
        var currentBuffer = graph.Buffers[i];

        var newOffset = streamToVbuffer[i].vBufOffset;
        if (newOffset != 0)
        {
          var diff = newOffset - currentBuffer.StartOffset;
          currentBuffer.StartOffset += diff;
          currentBuffer.EndOffset += diff;
        }
      }

      var runningSum = 0l;
      for(var i = 1; i < graph.BufferCount; i++)
      {
        var map = vBufferInfo[i - 1];
        runningSum += map.size;

        var prevBuffer = graph.Buffers[i - 1];
        var curBuffer = graph.Buffers[i];

        curBuffer.EndOffset = runningSum;
        if (i > 1)
          curBuffer.StartOffset = prevBuffer.EndOffset;
      }
    }

    return graph;
  }

  #endregion

  #region Property Read Methods

  private void ReadObjectsProperty(NativeReader reader, objGEOM_MNG graph, ISerializationContext context)
  {
    graph.objects = Serializer<List<objOBJ>>.Deserialize(reader, context);
  }

  //private void ReadObjectPropsProperty(NativeReader reader, objGEOM_MNG graph, ISerializationContext context)
  //{
  //  var count = reader.ReadInt16();

  //  for (var i = 0; i < count; i++)
  //  {
  //    graph.objects[i].objProps = reader.ReadByte();
  //  }
  //}

  private void ReadObjectPropsProperty(NativeReader reader, objGEOM_MNG graph, ISerializationContext context)
  {
    // This section only seems to be present in ss_prop__h.tpl
    var count = reader.ReadInt32();

    if (reader.ReadByte() == 0)
      return;

    for (var i = 0; i < count; i++)
    {
      var objId = reader.ReadInt16();
      _ = reader.ReadInt64();
      _ = reader.ReadInt32();
      _ = reader.ReadInt16();
    }
  }

  private void ReadObjectPsProperty(NativeReader reader, objGEOM_MNG graph, ISerializationContext context)
  {
    var count = reader.ReadInt32();

    if (reader.ReadByte() == 0)
      return;

    for (var i = 0; i < count; i++)
    {
      var objectId = reader.ReadInt32();
      graph.objects[objectId].ps = reader.ReadLengthPrefixedString32();
    }

  }

  private void ReadLodRootsProperty(NativeReader reader, objGEOM_MNG graph, ISerializationContext context)
  {
    graph.lodRoots = Serializer<List<objLOD_ROOT>>.Deserialize(reader, context);
  }

  private void ReadVBufferMappingProperty(NativeReader reader, objGEOM_MNG graph, ISerializationContext context)
  {
    graph.vBufferMapping = Serializer<objGEOM_VBUFFER_MAPPING>.Deserialize(reader, context);
  }

  private void ReadNamedObjectNames(NativeReader reader, objGEOM_MNG graph, ISerializationContext context)
  {
    var count = reader.ReadInt32();
    if (reader.ReadByte() == 0)
      return;

    var namedObjectNames = graph.namedObjectsNames = new string[count];
    for (var i = 0; i < count; i++)
      namedObjectNames[i] = reader.ReadLengthPrefixedString32();
  }

  private void ReadNamedObjectIds(NativeReader reader, objGEOM_MNG graph, ISerializationContext context)
  {
    var count = reader.ReadInt32();
    var namedObjectIds = graph.namedObjectsIds = new ushort[count];
    for (var i = 0; i < count; i++)
      namedObjectIds[i] = reader.ReadUInt16();

    if (graph.namedObjectsNames != null)
    {
      for (var i = 0; i < count; i++)
      {
        var id = graph.namedObjectsIds[i];
        var name = graph.namedObjectsNames[i];
        var obj = graph.objects[id];

        ASSERT(id == obj.id);
        if (obj.UnkName is null)
          obj.UnkName = name;
        if (obj.ReadName is null)
          obj.ReadName = name;
        if (obj.ReadOnlyName is null)
          obj.ReadOnlyName = name;
        if (obj.ReadOnlyObjectName is null)
          obj.ReadOnlyObjectName = name;
      }
    }
  }

  private void ReadMatrixLT(NativeReader reader, objGEOM_MNG graph, ISerializationContext context)
  {
    var count = reader.ReadInt32();
    var matr = graph.matrLT = new System.Numerics.Matrix4x4[count];

    for (var i = 0; i < count; i++)
      matr[i] = reader.ReadMatrix4x4();

    for (var i = 0; i < count; i++)
      graph.objects[i].MatrixLT = matr[i];
  }

  private void ReadMatrixModel(NativeReader reader, objGEOM_MNG graph, ISerializationContext context)
  {
    var count = reader.ReadInt32();
    var matr = graph.matrModel = new System.Numerics.Matrix4x4[count];

    for (var i = 0; i < count; i++)
      matr[i] = reader.ReadMatrix4x4();

    for (var i = 0; i < count; i++)
      graph.objects[i].MatrixModel = matr[i];
  }

  private void ReadObjectSplitInfo(NativeReader reader, objGEOM_MNG graph, ISerializationContext context)
  {
    graph.objSplitInfo = Serializer<List<objSPLIT_RANGE>>.Deserialize(reader, context);
  }


  #endregion

  #region Data Read Methods

  private void ReadData(NativeReader reader, objGEOM_MNG graph, ISerializationContext context)
  {
    /* Reads the geometry data.
     * NOTE: Everything inside of here is subjectively named and interpreted.
     * As of writing this method, there is very little material to go on in terms
     * of RTTI or strings.
     */

    if (reader.Position == reader.BaseStream.Length)
      return;

    while (true)
    {
      var sentinel = reader.ReadUInt16();

      switch (sentinel)
      {
        case objGEOM_MNGSentinels.Header:
          ReadHeaderData(reader, graph); //_ReadInfo
          break;
        case objGEOM_MNGSentinels.Unk_05: //_ReadGeomRefs
          ReadUnk5Data(reader, graph);
          break;
        case objGEOM_MNGSentinels.Buffers:
          ReadBufferData(reader, graph, context); //_ReadStreams
          break;
        case objGEOM_MNGSentinels.Meshes: //_ReadGeoms
          ReadMeshData(reader, graph, context);
          break;
        case objGEOM_MNGSentinels.SubMeshes: //_ReadSplits
          ReadSubMeshData(reader, graph, context);
          break;
        case objGEOM_MNGSentinels.EndOfData:
          ReadEndOfData(reader, graph);
          return;
        default:
          FAIL("Unknown objGEOM_MNG Data Sentinel: {0:X}", sentinel);
          break;
      }
    }
  }

  private void ReadHeaderData(NativeReader reader, objGEOM_MNG graph)
  {
    var endOffset = reader.ReadUInt32();

    graph.RootNodeIndex = reader.ReadInt16(); //rootObjId
    graph.NodeCount = reader.ReadInt32();
    graph.BufferCount = reader.ReadInt32();
    graph.MeshCount = reader.ReadInt32();
    graph.SubMeshCount = reader.ReadInt32();

    var unk_01 = reader.ReadUInt32(); // objPs count
    var unk_02 = reader.ReadUInt32(); // unk return value, not used?

    ASSERT(reader.Position == endOffset,
        "Reader position does not match data header's end offset.");
  }

  private void ReadUnk5Data(NativeReader reader, objGEOM_MNG graph)
  {
    var endOffset = reader.ReadUInt32();

    //while (reader.Position < endOffset)
    //{
    //  var sentinel = reader.ReadUInt16();
    //  var innerEndOffset = reader.ReadInt32();
    //  switch (sentinel)
    //  {
    //    case 0:
    //      ReadUnk5_0Data(reader, graph);
    //      break;
    //    case 2:
    //      ReadUnk5_2Data(reader, graph);
    //      break;
    //    default:
    //      FAIL("Unknown Unk5 Data Sentinel: {0:X}", sentinel);
    //      break;
    //  }

    //  ASSERT(reader.Position == innerEndOffset,
    //    "Reader position does not match data's end offset.");
    //}

    reader.Position = endOffset;

    ASSERT(reader.Position == endOffset,
        "Reader position does not match data's end offset.");
  }

  private void ReadUnk5_0Data(NativeReader reader, objGEOM_MNG graph)
  {
    var unk_1 = reader.ReadInt32();
    var unk_2 = reader.ReadInt32();
    var unk_3 = reader.ReadInt32();

  }

  private void ReadUnk5_2Data(NativeReader reader, objGEOM_MNG graph)
  {

  }

  private void ReadBufferData(NativeReader reader, objGEOM_MNG graph, ISerializationContext context)
  {
    graph.Buffers = Serializer<List<GeometryBuffer>>.Deserialize(reader, context);
  }

  private void ReadMeshData(NativeReader reader, objGEOM_MNG graph, ISerializationContext context)
  {
    graph.Meshes = Serializer<List<GeometryMesh>>.Deserialize(reader, context);
  }

  private void ReadSubMeshData(NativeReader reader, objGEOM_MNG graph, ISerializationContext context)
  {
    graph.SubMeshes = Serializer<List<GeometrySubMesh>>.Deserialize(reader, context);
  }

  private void ReadEndOfData(NativeReader reader, objGEOM_MNG graph)
  {
    var endOffset = reader.ReadUInt32();
    ASSERT(reader.Position == endOffset,
        "Reader position does not match data's end offset.");
  }

  #endregion

  #region Embedded Types

  private enum objGEOM_MNGType : ushort
  {
    Default = 1,
    Props = 3,
    Grass = 4
  }

  private class objGEOM_MNGSentinels
  {
    public const ushort Header = 0x0000;
    public const ushort Buffers = 0x0002;
    public const ushort Meshes = 0x0003;
    public const ushort SubMeshes = 0x0004;
    public const ushort Unk_05 = 0x0005;
    public const ushort EndOfData = 0xFFFF;
  }

  [Flags]
  public enum objGEOM_MNGFlags
  {
    Objects = 1 << 0,
    ObjectProps = 1 << 1,
    ObjectPS = 1 << 2,
    LodRoots = 1 << 3,
    VBufferMapping = 1 << 4,
    NamedObjectNames = 1 << 5,
    NamedObjectIds = 1 << 6,
    MatrixLT = 1 << 7,
    MatrixModel = 1 << 8,
    SplitInfo = 1 << 9
  }

  #endregion

}
