using LibSaber.Shared.Structures;

namespace LibSaber.SpaceMarine2.Structures;

public class GeometryMesh
{

  #region Properties

  public uint Id { get; set; }
  public BitSet<short> Flags { get; set; }
  public GeometryMeshBufferInfo[] Buffers { get; set; }

  #endregion

}

public struct GeometryMeshBufferInfo
{
  public int BufferId;
  public int SubBufferOffset;
}
