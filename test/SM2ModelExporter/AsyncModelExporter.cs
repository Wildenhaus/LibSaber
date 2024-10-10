using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Assimp;
using LibSaber.Common;
using LibSaber.Extensions;
using LibSaber.FileSystem;
using LibSaber.IO;
using LibSaber.SpaceMarine2.Enumerations;
using LibSaber.SpaceMarine2.Serialization;
using LibSaber.SpaceMarine2.Serialization.Geometry;
using LibSaber.SpaceMarine2.Structures;
using LibSaber.SpaceMarine2.Structures.Geometry;
using static LibSaber.Assertions;

namespace SM2ModelExporter;

public static class AsyncModelExporter
{

  public static Task ExportModel(IFileSystem fileSystem, string tplName, string outPath)
  {
    var tplFile = fileSystem.EnumerateFiles().SingleOrDefault(x => Path.GetFileName(x.Name) == tplName);
    ASSERT(tplFile is not null, $"{tplName} not found");

    return ExportModel(fileSystem, tplFile, outPath);
  }

  public static Task ExportModel(IFileSystem fileSystem, IFileSystemNode tplNode, string outPath)
  {
    var reader = new NativeReader(tplNode.Open(), Endianness.LittleEndian);
    var tpl = Serializer<animTPL>.Deserialize(reader);

    IFileSystemNode dataNode = tplNode;

    var tplDataNodeName = Path.ChangeExtension(tplNode.Name, ".tpl_data");
    var tplDataNode = fileSystem.EnumerateFiles().SingleOrDefault(x => x.Name == tplDataNodeName);
    if (tplDataNode is not null)
      dataNode = tplDataNode;

    var name = Path.GetFileNameWithoutExtension(tplNode.Name);
    var request = new AsyncModelExportJob(name, tpl.GeometryGraph, dataNode, outPath);
    return ExportModel(request);
  }

  public static Task ExportModel(AsyncModelExportJob request)
  {
    return request.Run();
  }

}

public class AsyncModelExportJob
{

  public AsyncSceneContext Context { get; }
  public string OutPath { get; }

  public AsyncModelExportJob(string name, objGEOM_MNG graph, IFileSystemNode dataNode, string outPath)
  {
    OutPath = outPath;
    Context = new AsyncSceneContext(name, graph, dataNode);
  }

  public async Task Run()
  {
    await ConvertObjects();

    ExportFile();
  }

  private void ExportFile()
  {
    if (string.IsNullOrEmpty(OutPath))
      return;

    using (var ctx = new AssimpContext())
    {
      var path = Path.Combine(OutPath, Context.Name) + ".fbx";
      if (File.Exists(path))
        File.Delete(path);

      Console.WriteLine("Writing file: {0}...", path);

      const PostProcessSteps PP_STEPS = //PostProcessSteps.None;
        PostProcessSteps.FindInvalidData |
        PostProcessSteps.GlobalScale |
        PostProcessSteps.OptimizeGraph |
        PostProcessSteps.OptimizeMeshes |
        PostProcessSteps.ValidateDataStructure;

      var success = ctx.ExportFile(Context.Scene, path, "fbx", PP_STEPS);
      ASSERT(success, "Export failed.");

      Console.WriteLine("Done.");

      //var test = ctx.ImportFile(path);
    }
  }

  private async Task ConvertObjects()
  {
    AddNodes(Context.GeometryGraph.objects);
    await AddMeshes();
  }

  #region Nodes

  private void AddNodes(List<objOBJ> objects)
  {
    Console.WriteLine("Adding Nodes...");
    var rootNode = Context.RootNode;
    var rootObject = Context.GeometryGraph.RootObject;
    if (rootObject.ReadName is null)
      rootObject.ReadName = Context.Name;

    AddNodesRecursive(rootObject, rootNode);
  }

  private void AddNodesRecursive(objOBJ obj, Node parentNode)
  {
    var node = CreateNode(obj, parentNode);

    foreach (var childObj in obj.EnumerateChildren())
      AddNodesRecursive(childObj, node);
  }

  private Node CreateNode(objOBJ obj, Node parentNode)
  {
    var objName = obj.GetName();

    var node = new Node(objName, parentNode);
    if (parentNode is not null)
      parentNode.Children.Add(node);

    Context.Nodes.Add(obj.id, node);
    Context.NodeNames[objName] = node;

    var matrix = obj.MatrixLT;
    if (!obj.SubMeshes.Any())
      matrix = obj.MatrixModel;

    var transform = matrix.ToAssimp();
    transform.Transpose();
    node.Transform = transform;

    return node;
  }

  #endregion

  #region Meshes

  private async Task AddMeshes()
  {
    var meshLookup = await CreateMeshes();

    var scene = Context.Scene;
    foreach (var pair in meshLookup)
    {
      var objId = pair.Key;
      var meshes = pair.Value;

      var objNode = Context.Nodes[objId];
      foreach (var mesh in meshes)
      {
        var meshIndex = scene.MeshCount;
        scene.Meshes.Add(mesh);
        objNode.MeshIndices.Add(meshIndex);
      }
    }
  }

  private async Task<Dictionary<short, Mesh[]>> CreateMeshes()
  {
    var meshLookup = new Dictionary<short, Mesh[]>();
    var jobParams = new List<BuildMeshParams>();

    foreach (var obj in Context.GeometryGraph.objects)
    {
      if (!obj.SubMeshes.Any())
        continue;

      int submeshCount = 0;
      foreach (var submesh in obj.SubMeshes)
        jobParams.Add(new(obj, submeshCount++, submesh));

      meshLookup.Add(obj.id, new Mesh[submeshCount]);
    }

    var buildSubMeshBlock = new ActionBlock<BuildMeshParams>(parameters =>
    {
      var (obj, index, submesh) = parameters;

      var builtMesh = AsyncSubMeshBuilder.Build(obj, submesh, Context);
      meshLookup[obj.id][index] = builtMesh;
    }, new() { EnsureOrdered = false, MaxDegreeOfParallelism = 4 });

    foreach (var param in jobParams)
      buildSubMeshBlock.Post(param);

    buildSubMeshBlock.Complete();
    await buildSubMeshBlock.Completion;

    return meshLookup;
  }

  #endregion

  record BuildMeshParams(objOBJ Object, int SubmeshIndex, GeometrySubMesh submesh);

}

public class AsyncSceneContext
{

  private byte[] _data;

  public string Name { get; }

  public Scene Scene { get; }
  public Node RootNode => Scene.RootNode;

  public IFileSystemNode DataNode { get; }
  public objGEOM_MNG GeometryGraph { get; }

  public Dictionary<short, Node> Nodes { get; }
  public Dictionary<string, Node> NodeNames { get; }

  public AsyncSceneContext(string name, objGEOM_MNG geometryGraph, IFileSystemNode dataNode)
  {
    Name = name;

    Scene = new Scene();
    Scene.RootNode = new Node(string.Empty);

    DataNode = dataNode;
    GeometryGraph = geometryGraph;

    Nodes = new();
    NodeNames = new();

    Scene.Materials.Add(new Material { Name = "DefaultMaterial" });
  }

  public Stream GetDataStream() => DataNode.Open();
  public NativeReader GetReader() => new(new MemoryStream(GetBufferData()), bufferSize: 512 * 1024);

  public unsafe SpanReader GetSpanReader(long startOffset, int length)
  {
    var data = GetBufferData();

    fixed (byte* ptr = data)
    {
      var span = new Span<byte>(ptr + startOffset, length);
      return new SpanReader(span);
    }
  }

  public byte[] GetBufferData()
  {
    lock (this)
    {
      if (_data is null)
      {
        var stream = GetDataStream();

        if (stream is MemoryStream ms)
        {
          _data = ms.ToArray();
        }
        else
        {
          _data = new byte[stream.Length];
          stream.Read(_data);
        }
      }
    }

    return _data;
  }


}

internal class AsyncSubMeshBuilder
{

  private AsyncSceneContext Context { get; }
  private objGEOM_MNG Graph => Context.GeometryGraph;
  private NativeReader Reader => Context.GetReader();

  private objOBJ Object { get; }
  private GeometrySubMesh Submesh { get; }

  private string Name { get; }
  private Mesh Mesh { get; }

  private Dictionary<short, Bone> Bones { get; }
  private Dictionary<string, Bone> BoneNames { get; }
  private Dictionary<int, int> VertexLookup { get; }

  private AsyncSubMeshBuilder(objOBJ obj, GeometrySubMesh submesh, AsyncSceneContext context)
  {
    Object = obj;
    Submesh = submesh;
    Context = context;

    Name = obj.GetName();
    Mesh = new Mesh(Name, PrimitiveType.Triangle);

    Bones = new Dictionary<short, Bone>();
    BoneNames = new Dictionary<string, Bone>();
    VertexLookup = new Dictionary<int, int>();
  }

  public static Mesh Build(objOBJ obj, GeometrySubMesh submesh, AsyncSceneContext context)
  {
    var builder = new AsyncSubMeshBuilder(obj, submesh, context);
    return builder.Build();
  }

  private Mesh Build()
  {
    var meshData = Graph.Meshes[Submesh.MeshId];
    foreach (var bufferInfo in meshData.Buffers)
    {
      var buffer = Graph.Buffers[bufferInfo.BufferId];

      switch (buffer.ElementType)
      {
        case GeometryElementType.Vertex:
          AddVertices(buffer, bufferInfo);
          break;
        case GeometryElementType.Face:
          if (buffer.ElementSize != 6)
            continue;
          AddFaces(buffer, bufferInfo);
          break;
        case GeometryElementType.Interleaved:
          AddInterleavedData(buffer, bufferInfo);
          break;
      }
    }

    return Mesh;
  }

  #region Face Methods

  private void AddFaces(GeometryBuffer buffer, GeometryMeshBufferInfo bufferInfo)
  {
    Reader.Position = buffer.StartOffset;
    var offset = Submesh.BufferInfo.FaceOffset;
    var startIndex = offset + bufferInfo.SubBufferOffset / buffer.ElementSize;
    var endIndex = startIndex + Submesh.BufferInfo.FaceCount;

    var startOffset = buffer.StartOffset + (startIndex * buffer.ElementSize);
    var endOffset = startOffset + (Submesh.BufferInfo.FaceCount * buffer.ElementSize);
    var length = endOffset - startOffset;
    var count = Submesh.BufferInfo.FaceCount;

    //var faceSerializer = new FaceSerializer(buffer);
    //foreach (var face in faceSerializer.DeserializeRange(Reader, startIndex, endIndex))
    var reader = Context.GetSpanReader(startOffset, (int)length);
    for (var i = 0; i < count; i++)
    {
      var assimpFace = new Assimp.Face();
      assimpFace.Indices.Add(VertexLookup[reader.ReadUInt16()]);
      assimpFace.Indices.Add(VertexLookup[reader.ReadUInt16()]);
      assimpFace.Indices.Add(VertexLookup[reader.ReadUInt16()]);

      Mesh.Faces.Add(assimpFace);
    }
  }

  #endregion

  #region Vertex Methods

  private void AddVertices(GeometryBuffer buffer, GeometryMeshBufferInfo bufferInfo)
  {
    //Reader.Position = buffer.StartOffset;
    var offset = Submesh.BufferInfo.VertexOffset;
    var startIndex = offset + bufferInfo.SubBufferOffset / buffer.ElementSize;
    var endIndex = startIndex + Submesh.BufferInfo.VertexCount;

    var startOffset = buffer.StartOffset + (startIndex * buffer.ElementSize);
    var endOffset = startOffset + (Submesh.BufferInfo.VertexCount * buffer.ElementSize);
    var length = endOffset - startOffset;

    //var scale = new Vector3D(1, 1, 1);
    //if (Submesh.Scale.HasValue)
    //  scale = Submesh.Scale.Value.ToAssimp();

    //var pos = new Vector3D(0, 0, 0);
    //if (Submesh.Position.HasValue)
    //  pos = Submesh.Position.Value.ToAssimp();

    var reader = Context.GetSpanReader(startOffset, (int)length);
    var vertexSerializer = new VertexSerializer(buffer, Submesh.Position, Submesh.Scale);
    var enumerator = vertexSerializer.GetEnumerator(reader, startIndex, endIndex);
    while (enumerator.MoveNext())
    {
      var vertex = enumerator.Current;
      Mesh.Vertices.Add(vertex.Position.ToAssimpVector3D());

      if (vertex.HasNormal)
        Mesh.Normals.Add(vertex.Normal.ToAssimpVector3D());

      if (vertex.HasSkinningData)
        AddVertexSkinningData(vertex);

      VertexLookup.Add(offset++, VertexLookup.Count);
    }
  }

  private void AddVertexSkinningData(Vertex vertex)
  {
    var boneIds = Submesh.BoneIds;
    var set = new HashSet<short>();

    if (vertex.HasWeight1 && set.Add(vertex.Index1))
      AddVertexWeight(vertex.Index1, vertex.Weight1);
    else return;

    if (vertex.HasWeight2 && set.Add(vertex.Index2))
      AddVertexWeight(vertex.Index2, vertex.Weight2);
    else return;

    if (vertex.HasWeight3 && set.Add(vertex.Index3))
      AddVertexWeight(vertex.Index3, vertex.Weight3);
    else return;

    //if (vertex.HasWeight4 && set.Add(vertex.Index4))
    //  AddVertexWeight(vertex.Index4, vertex.Weight4);
    //else return;

  }

  private void AddVertexWeight(short boneObjectId, float weight, int vertIndex = -1)
  {
    if (boneObjectId == -1)
      return;

    if (vertIndex == -1)
      vertIndex = Mesh.VertexCount - 1;

    var bone = GetOrCreateBone(boneObjectId);
    bone.VertexWeights.Add(new VertexWeight(vertIndex, weight));
  }

  private Bone GetOrCreateBone(short boneObjectId)
  {
    var boneObject = Graph.objects[boneObjectId];
    var boneName = boneObject.GetName();
    if (boneName is null)
      boneName = $"Bone{boneObjectId}";

    if (!BoneNames.TryGetValue(boneName, out var bone))
    {
      System.Numerics.Matrix4x4.Invert(boneObject.MatrixLT, out var invMatrix);
      var transform = invMatrix.ToAssimp();
      transform.Transpose();

      bone = new Bone
      {
        Name = boneName,
        OffsetMatrix = transform
      };

      Mesh.Bones.Add(bone);
      Bones.Add(boneObjectId, bone);
      BoneNames.Add(boneName, bone);
    }

    return bone;
  }

  public void ParentMeshToBone(objOBJ boneObject)
  {
    if (boneObject.GetName() is null)
      return;

    for (var i = 0; i < Mesh.VertexCount; i++)
      AddVertexWeight(boneObject.id, 1, i);
  }

  #endregion

  #region Interleaved Methods

  private void AddInterleavedData(GeometryBuffer buffer, GeometryMeshBufferInfo bufferInfo)
  {
    //Reader.Position = buffer.StartOffset;
    var offset = Submesh.BufferInfo.VertexOffset;
    var startIndex = offset + (bufferInfo.SubBufferOffset / buffer.ElementSize);
    var endIndex = startIndex + Submesh.BufferInfo.VertexCount;

    var startOffset = buffer.StartOffset + (startIndex * buffer.ElementSize);
    var endOffset = startOffset + (Submesh.BufferInfo.VertexCount * buffer.ElementSize);
    var length = endOffset - startOffset;

    var reader = Context.GetSpanReader(startOffset, (int)length);

    var interleavedSerializer = new InterleavedDataSerializer(buffer);
    var enumerator = interleavedSerializer.GetEnumerator(reader, startIndex, endIndex);
    while (enumerator.MoveNext())
    {
      var datum = enumerator.Current;
      if (datum.HasUV0) AddVertexUV(0, datum.UV0);
      if (datum.HasUV1) AddVertexUV(1, datum.UV1);
      if (datum.HasUV2) AddVertexUV(2, datum.UV2);
      if (datum.HasUV3) AddVertexUV(3, datum.UV3);
      if (datum.HasUV4) AddVertexUV(4, datum.UV4);

      // TODO: Assimp only allows 1 tangent channel.
      if (datum.HasTangent0) AddVertexTangent(0, datum.Tangent0);

      if (datum.HasColor0) AddVertexColor(0, datum.Color0);
      if (datum.HasColor1) AddVertexColor(1, datum.Color1);
      if (datum.HasColor2) AddVertexColor(2, datum.Color2);
    }
  }

  private void AddVertexTangent(byte channel, Vector4 tangentVector)
  {
    Mesh.Tangents.Add(tangentVector.ToAssimpVector3D());
  }

  private void AddVertexUV(byte channel, Vector4 uvVector)
  {
    if (!Submesh.UvScaling.TryGetValue(channel, out var scaleFactor))
      scaleFactor = 1;

    var scaleVector = new Vector3D(scaleFactor);
    var scaledUvVector = uvVector.ToAssimpVector3D() * scaleVector;

    Mesh.TextureCoordinateChannels[channel].Add(scaledUvVector);
    Mesh.UVComponentCount[channel] = 2;
  }

  private void AddVertexColor(byte channel, Vector4 colorVector)
  {
    var color = colorVector.ToAssimpColor4D();
    Mesh.VertexColorChannels[channel].Add(color);
  }

  #endregion

}