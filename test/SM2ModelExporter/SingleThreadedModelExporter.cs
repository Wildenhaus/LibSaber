using System.Diagnostics;
using System.Numerics;
using Assimp;
using LibSaber.FileSystem;
using LibSaber.IO;
using LibSaber.SpaceMarine2.Enumerations;
using LibSaber.SpaceMarine2.Serialization;
using LibSaber.SpaceMarine2.Serialization.Geometry;
using LibSaber.SpaceMarine2.Structures;
using LibSaber.SpaceMarine2.Structures.Geometry;
using static LibSaber.Assertions;

namespace SM2ModelExporter;

public static class SingleThreadedModelExporter
{

  public static void ExportModel(IFileSystem fileSystem, string tplName, string outPath)
  {
    var tplFile = fileSystem.EnumerateFiles().SingleOrDefault(x => Path.GetFileName(x.Name) == tplName);
    ASSERT(tplFile is not null, $"{tplName} not found");

    ExportModel(fileSystem, tplFile, outPath);
  }

  public static void ExportModel(IFileSystem fileSystem, IFileSystemNode tplNode, string outPath)
  {
    var reader = new NativeReader(tplNode.Open(), Endianness.LittleEndian);
    var tpl = Serializer<animTPL>.Deserialize(reader);

    Stream dataStream = null;

    var tplDataNodeName = Path.ChangeExtension(tplNode.Name, ".tpl_data");
    var tplDataNode = fileSystem.EnumerateFiles().SingleOrDefault(x => x.Name == tplDataNodeName);
    if (tplDataNode is not null)
      dataStream = tplDataNode.Open();
    else
      dataStream = tplNode.Open();

    ASSERT(dataStream is not null, "Data stream is null");

    var name = Path.GetFileNameWithoutExtension(tplNode.Name);
    var request = new SingleThreadedModelExportJob(name, tpl.GeometryGraph, dataStream, outPath);
    request.Context.AddLodDefinitions(tpl.lodDef);
    ExportModel(request);
  }

  public static void ExportModel(SingleThreadedModelExportJob request)
  {
    request.Run();
  }

}

public class SingleThreadedModelExportJob
{
  public SingleThreadedSceneContext Context { get; }
  public string OutPath { get; }

  public SingleThreadedModelExportJob(string name, objGEOM_MNG graph, Stream dataStream, string outPath)
  {
    OutPath = outPath;
    Context = new SingleThreadedSceneContext(name, graph, dataStream);
  }

  public void Run()
  {
    Console.WriteLine("Exporting {0}...", Context.Name);

    ConvertObjects();

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

      const PostProcessSteps PP_STEPS = PostProcessSteps.None;
      //PostProcessSteps.FindInvalidData |
      //PostProcessSteps.GlobalScale |
      //PostProcessSteps.OptimizeGraph |
      //PostProcessSteps.OptimizeMeshes |
      //PostProcessSteps.ValidateDataStructure;

      var success = ctx.ExportFile(Context.Scene, path, "fbx", PP_STEPS);
      ASSERT(success, "Export failed.");

      Console.WriteLine("Done.");

      //var test = ctx.ImportFile(path);
    }
  }

  private void ConvertObjects()
  {
    AddNodes(Context.GeometryGraph.objects);
    //BuildSkinCompounds();
    AddMeshNodes(Context.GeometryGraph.objects);
    //AddRemainingMeshBones();
  }

  private void BuildSkinCompounds()
  {
    var skinCompoundIds = Context.GeometryGraph.SubMeshes
      .Select(x => x.BufferInfo.SkinCompoundId)
      .Where(x => x >= 0)
      .Distinct()
      .ToArray();

    if (!skinCompoundIds.Any())
      return;

    Console.WriteLine("Building Skin Compounds...");

    foreach (var skinCompoundId in skinCompoundIds)
    {
      var skinCompoundObject = Context.GeometryGraph.objects[skinCompoundId];
      if (!skinCompoundObject.SubMeshes.Any())
        continue;

      var builder = new SingleThreadedMeshBuilder(Context, skinCompoundObject, skinCompoundObject.SubMeshes.First());
      builder.Build();

      Context.SkinCompounds[skinCompoundId] = builder;
    }
  }

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

    var transform = obj.MatrixModel.ToAssimp();
    transform.Transpose();
    node.Transform = transform;

    return node;
  }

  private void AddMeshNodes(List<objOBJ> objects)
  {
    Console.WriteLine("Converting Meshes...");

    var objectsWithMeshes = objects.Where(x => x.SubMeshes.Any()).ToArray();
    var submeshCount = objectsWithMeshes.Sum(x => x.SubMeshes.Count());

    foreach (var obj in objectsWithMeshes)
    {
      //if (Context.LodIndices.TryGetValue(obj.id, out var lodIndex))
      //{
      //  if (lodIndex != 0)
      //    continue;
      //}

      //if (Context.SkinCompounds.ContainsKey(obj.id))
      //  continue;

      AddSubMeshes(obj);
    }
  }

  private void AddSubMeshes(objOBJ obj)
  {
    foreach (var submesh in obj.SubMeshes)
    {
      var node = new Node(obj.GetName(), Context.RootNode);
      Context.RootNode.Children.Add(node);

      var builder = new SingleThreadedMeshBuilder(Context, obj, submesh);
      var mesh = builder.Build();

      Context.Scene.Meshes.Add(mesh);
      var meshId = Context.Scene.Meshes.Count - 1;
      node.MeshIndices.Add(meshId);

      var transform = obj.MatrixLT.ToAssimp();
      transform.Transpose();
      node.Transform = transform;

      var meshName = obj.GetName();
      //if (!mesh.HasBones && obj.Parent != null)
      //  builder.ParentMeshToBone(obj.Parent);
    }
  }

  private void AddRemainingMeshBones()
  {
    // Blender sometimes freaks out if bones in the hierarchy chain aren't on the meshes.
    // Hence this icky looking method.

    Console.WriteLine("Adding remaining bones...");

    var boneLookup = new Dictionary<string, Bone>();
    foreach (var mesh in Context.Scene.Meshes)
    {
      foreach (var bone in mesh.Bones)
        if (!boneLookup.ContainsKey(bone.Name))
          boneLookup.Add(bone.Name, new Bone { Name = bone.Name, OffsetMatrix = bone.OffsetMatrix });
    }

    foreach (var mesh in Context.Scene.Meshes)
    {
      foreach (var meshBone in mesh.Bones.ToList())
      {
        var meshBoneNode = Context.Scene.RootNode.FindNode(meshBone.Name);
        if (meshBoneNode is null)
          continue;

        var parent = meshBoneNode.Parent;
        while (parent != null && !parent.HasMeshes)
        {
          if (!mesh.Bones.Any(x => x.Name == parent.Name))
            if (boneLookup.TryGetValue(parent.Name, out var parentBone))
              mesh.Bones.Add(new Bone { Name = parentBone.Name, OffsetMatrix = parentBone.OffsetMatrix });

          parent = parent.Parent;
        }
      }
    }
  }

}

#region Scene Context

public class SingleThreadedSceneContext
{

  #region Properties

  public Scene Scene { get; }
  public string Name { get; }
  public Node RootNode => Scene.RootNode;

  public Stream Stream { get; }
  public NativeReader Reader { get; }
  public objGEOM_MNG GeometryGraph { get; }

  public Dictionary<short, Bone> Bones { get; }
  public Dictionary<short, Node> Nodes { get; }
  public Dictionary<string, Node> NodeNames { get; }
  public Dictionary<string, int> MaterialIndices { get; }
  public Dictionary<short, SingleThreadedMeshBuilder> SkinCompounds { get; }
  public Dictionary<short, short> LodIndices { get; }

  #endregion

  #region Constructor

  public SingleThreadedSceneContext(string name, objGEOM_MNG graph, Stream stream)
  {
    Name = name;
    Scene = new Scene();
    Scene.RootNode = new Node(string.Empty);

    Stream = stream;
    Reader = new NativeReader(stream, Endianness.LittleEndian);
    GeometryGraph = graph;

    Bones = new Dictionary<short, Bone>();
    Nodes = new Dictionary<short, Node>();
    NodeNames = new Dictionary<string, Node>();
    MaterialIndices = new Dictionary<string, int>();
    SkinCompounds = new Dictionary<short, SingleThreadedMeshBuilder>();
    LodIndices = new Dictionary<short, short>();

    Scene.Materials.Add(new Material() { Name = "DefaultMaterial" });
  }

  public void AddLodDefinitions(IList<tplLOD_DEF> lodDefs)
  {
    if (lodDefs is null)
      return;

    foreach (var lodDefinition in lodDefs)
      LodIndices.Add((short)lodDefinition.objId, (short)lodDefinition.index);
  }

  #endregion

}

#endregion

#region SingleThreadedMeshBuilder

public class SingleThreadedMeshBuilder
{

  #region Properties

  protected SingleThreadedSceneContext Context { get; }
  protected objOBJ Object { get; }
  protected GeometrySubMesh Submesh { get; }

  private Stream Stream => Context.Stream;
  private NativeReader Reader => Context.Reader;
  private objGEOM_MNG Graph => Context.GeometryGraph;

  public Mesh Mesh { get; }
  public short SkinCompoundId { get; }
  protected Dictionary<short, Bone> Bones { get; }
  protected Dictionary<string, Bone> BoneNames { get; }
  protected Dictionary<int, int> VertexLookup { get; }

  #endregion

  #region Constructor

  public SingleThreadedMeshBuilder(SingleThreadedSceneContext context, objOBJ obj, GeometrySubMesh submesh)
  {
    Context = context;
    Object = obj;
    Submesh = submesh;
    SkinCompoundId = Submesh.BufferInfo.SkinCompoundId;

    var meshName = Object.GetName();
    Mesh = new Mesh(meshName, PrimitiveType.Triangle);

    Bones = new Dictionary<short, Bone>();
    BoneNames = new Dictionary<string, Bone>();
    VertexLookup = new Dictionary<int, int>();
  }

  #endregion

  #region Public Methods

  public Mesh Build()
  {
    var meshData = Graph.Meshes[Submesh.MeshId];
    foreach (var meshBuffer in meshData.Buffers)
    {
      var buffer = Graph.Buffers[meshBuffer.BufferId];

      switch (buffer.ElementType)
      {
        case GeometryElementType.Vertex:
          AddVertices(buffer, meshBuffer);
          break;
        case GeometryElementType.Face:
          if (buffer.ElementSize != 6) // TODO: What is this buffer? Not faces. thunderhawk_body.tpl
            continue;
          AddFaces(buffer, meshBuffer);
          break;
        case GeometryElementType.Interleaved:
          AddInterleavedData(buffer, meshBuffer);
          break;
          //case GeometryElementType.BoneId:
          //  AddSkinCompoundBoneIds( buffer, meshBuffer );
          //  break;
      }
    }

    //ApplySkinCompoundData();
    //AddMaterial();

    return Mesh;
  }

  public void ParentMeshToBone(objOBJ boneObject)
  {
    if (boneObject.GetName() is null)
      return;

    for (var i = 0; i < Mesh.VertexCount; i++)
      AddVertexWeight(boneObject.id, 1, i);
  }

  #endregion

  #region Face Methods

  private void AddFaces(GeometryBuffer buffer, GeometryMeshBufferInfo bufferInfo)
  {
    Reader.Position = buffer.StartOffset;
    var offset = Submesh.BufferInfo.FaceOffset;
    var startIndex = offset + bufferInfo.SubBufferOffset / buffer.ElementSize;
    var endIndex = startIndex + Submesh.BufferInfo.FaceCount;

    var faceSerializer = new FaceSerializer(buffer);
    foreach (var face in faceSerializer.DeserializeRange(Reader, startIndex, endIndex))
    {
      var assimpFace = new Assimp.Face();
      assimpFace.Indices.Add(VertexLookup[face.A]);
      assimpFace.Indices.Add(VertexLookup[face.B]);
      assimpFace.Indices.Add(VertexLookup[face.C]);

      Mesh.Faces.Add(assimpFace);
    }
  }

  #endregion

  #region Vertex Methods

  private void AddVertices(GeometryBuffer buffer, GeometryMeshBufferInfo bufferInfo)
  {
    Reader.Position = buffer.StartOffset;
    var offset = Submesh.BufferInfo.VertexOffset;
    var startIndex = offset + bufferInfo.SubBufferOffset / buffer.ElementSize;
    var endIndex = startIndex + Submesh.BufferInfo.VertexCount;

    var scale = new Vector3D(1, 1, 1);
    if (Submesh.Scale.HasValue)
      scale = Submesh.Scale.Value.ToAssimp();

    var pos = new Vector3D(0, 0, 0);
    if (Submesh.Position.HasValue)
      pos = Submesh.Position.Value.ToAssimp();

    var vertexSerializer = new VertexSerializer(buffer);
    foreach (var vertex in vertexSerializer.DeserializeRange(Reader, startIndex, endIndex))
    {
      Mesh.Vertices.Add(vertex.Position.ToAssimpVector3D() * scale + pos);

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

  #endregion

  #region Interleaved Methods

  private void AddInterleavedData(GeometryBuffer buffer, GeometryMeshBufferInfo bufferInfo)
  {
    Reader.Position = buffer.StartOffset;
    var offset = Submesh.BufferInfo.VertexOffset;
    var startIndex = offset + (bufferInfo.SubBufferOffset / buffer.ElementSize);
    var endIndex = startIndex + Submesh.BufferInfo.VertexCount;

    var interleavedSerializer = new InterleavedDataSerializer(buffer);
    foreach (var datum in interleavedSerializer.DeserializeRange(Reader, startIndex, endIndex))
    {
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

  #region Skin Compound Methods

  private void AddSkinCompoundBoneIds(GeometryBuffer buffer, GeometryMeshBufferInfo bufferInfo)
  {
    var offset = Submesh.BufferInfo.VertexOffset;
    var startIndex = offset + bufferInfo.SubBufferOffset / buffer.ElementSize;
    var endIndex = startIndex + Submesh.BufferInfo.VertexCount;

    var boneIds = Submesh.BoneIds;

    Reader.Position = buffer.StartOffset + bufferInfo.SubBufferOffset + offset * buffer.ElementSize;
    for (var i = startIndex; i < endIndex; i++)
    {
      var boneIndex = boneIds[Reader.ReadInt32()];
      var vertIndex = VertexLookup[offset++];
      AddVertexWeight(boneIndex, 1, vertIndex);
    }
  }

  private void ApplySkinCompoundData()
  {
    if (SkinCompoundId == -1)
      return;

    if (!Context.SkinCompounds.TryGetValue(SkinCompoundId, out var skinCompound))
      return;

    var offset = Submesh.BufferInfo.VertexOffset;
    var vertCount = Submesh.BufferInfo.VertexCount;
    var skinCompoundVertOffset = skinCompound.VertexLookup.Min(x => x.Key);

    var boneLookup = new Dictionary<short, short>();
    foreach (var bonePair in skinCompound.Bones)
    {
      var boneId = bonePair.Key;
      var sourceBone = bonePair.Value;

      if (!boneLookup.TryGetValue(boneId, out var adjustedBoneId))
      {
        var boneObject = Graph.objects[boneId];
        var boneName = Object.GetName();
        if (boneName is null)
          adjustedBoneId = boneId;
        else if (boneObject.GetName() != boneName)
        {
          var parentBoneObject = Graph.objects.FirstOrDefault(x => x.GetName() == boneName);
          if (parentBoneObject != null)
          {
            boneObject = parentBoneObject;
            adjustedBoneId = boneObject.id;
          }
        }

        boneLookup.Add(boneId, adjustedBoneId);
      }

      foreach (var weight in sourceBone.VertexWeights)
      {
        var trueVertOffset = weight.VertexID + skinCompoundVertOffset;
        if (!VertexLookup.TryGetValue(trueVertOffset, out var translatedVertOffset))
          continue;

        var skinVertex = skinCompound.Mesh.Vertices[weight.VertexID];
        var targetVertex = Mesh.Vertices[translatedVertOffset];
        Debug.Assert(skinVertex.X == targetVertex.X);
        Debug.Assert(skinVertex.Y == targetVertex.Y);
        Debug.Assert(skinVertex.Z == targetVertex.Z);

        AddVertexWeight(adjustedBoneId, 1, translatedVertOffset);
      }
    }
  }

  #endregion

  #region Bone Methods

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

  private void AddMaterial()
  {
    var submeshMaterial = Submesh.Material;
    if (submeshMaterial is null)
      return;

    var materialName = submeshMaterial.ShadingMaterialMaterial;
    var textureName = submeshMaterial.ShadingMaterialTexture;
    var exportMatName = submeshMaterial.MaterialName;

    if (Context.MaterialIndices.TryGetValue(exportMatName, out var materialIndex))
    {
      Mesh.MaterialIndex = materialIndex;
      return;
    }

    var material = new Material { Name = exportMatName };
    material.TextureDiffuse = new TextureSlot(textureName, TextureType.Diffuse, 0, TextureMapping.FromUV, 0, blendFactor: 0, TextureOperation.Add, TextureWrapMode.Wrap, TextureWrapMode.Wrap, 0); ;
    material.TextureNormal = new TextureSlot($"{textureName}_nm", TextureType.Normals, 1, TextureMapping.FromUV, 0, blendFactor: 0, TextureOperation.Add, TextureWrapMode.Wrap, TextureWrapMode.Wrap, 0);
    material.TextureSpecular = new TextureSlot($"{textureName}_spec", TextureType.Specular, 2, TextureMapping.FromUV, 0, blendFactor: 0, TextureOperation.Add, TextureWrapMode.Wrap, TextureWrapMode.Wrap, 0);

    materialIndex = Context.Scene.Materials.Count;

    Context.Scene.Materials.Add(material);
    Context.MaterialIndices.Add(exportMatName, materialIndex);
    Mesh.MaterialIndex = materialIndex;
  }

  #endregion

}

#endregion