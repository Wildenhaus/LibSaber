using System.Numerics;

namespace LibSaber.SpaceMarine2.Structures;

public class objGEOM_MNG
{

  #region Properties

  public List<objOBJ> objects { get; set; }
  public string[] objProps { get; set; }
  public List<objLOD_ROOT> lodRoots { get; set; }

  public objGEOM_VBUFFER_MAPPING vBufferMapping { get; set; }
  public string[] namedObjectsNames { get; set; }
  public ushort[] namedObjectsIds { get; set; }
  public Matrix4x4[] matrLT { get; set; }
  public Matrix4x4[] matrModel { get; set; }
  public List<objSPLIT_RANGE> objSplitInfo { get; set; }

  public short RootNodeIndex { get; set; }
  public int NodeCount { get; set; }
  public int BufferCount { get; set; }
  public int MeshCount { get; set; }
  public int SubMeshCount { get; set; }

  public bool ContainsVertexBuffers { get; set; }

  public objOBJ RootObject => objects[RootNodeIndex];
  public List<GeometryBuffer> Buffers { get; set; }
  public List<GeometryMesh> Meshes { get; set; }
  public List<GeometrySubMesh> SubMeshes { get; set; }

  #endregion

}
