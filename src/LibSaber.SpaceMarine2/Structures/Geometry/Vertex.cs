using System.Numerics;

namespace LibSaber.SpaceMarine2.Structures.Geometry
{

  public struct Vertex
  {

    public Vector4 Position;
    public Vector4? Normal;

    public short Index1 { get; set; }
    public short Index2 { get; set; }
    public short Index3 { get; set; }
    public short Index4 { get; set; }

    public float? Weight1 { get; set; }
    public float? Weight2 { get; set; }
    public float? Weight3 { get; set; }
    public float? Weight4 { get; set; }

    public bool HasSkinningData
    {
      get => Weight1.HasValue;
    }

  }

}
