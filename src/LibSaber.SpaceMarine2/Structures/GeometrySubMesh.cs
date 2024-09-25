using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using LibSaber.SpaceMarine2.Structures.Materials;

namespace LibSaber.SpaceMarine2.Structures;

public class GeometrySubMesh
{

  #region Properties

  public GeometrySubMeshBufferInfo BufferInfo { get; set; }
  public int MeshId { get; set; } // TODO: This is a guess

  public Vector3? Position { get; set; }
  public Vector3? Scale { get; set; }

  public ushort NodeId { get; set; }
  public SaberMaterial Material { get; set; }

  public short[] BoneIds { get; set; }
  public Dictionary<byte, short> UvScaling { get; set; }

  #endregion

  #region

  public GeometrySubMesh()
  {
    UvScaling = new Dictionary<byte, short>();
  }

  #endregion

}

public struct GeometrySubMeshBufferInfo
{
  public ushort VertexOffset;
  public ushort VertexCount;
  public ushort FaceOffset;
  public ushort FaceCount;
  public short NodeId;
  public short SkinCompoundId;
}

