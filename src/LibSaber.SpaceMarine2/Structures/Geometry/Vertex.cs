using System.Numerics;

namespace LibSaber.SpaceMarine2.Structures.Geometry
{

  public struct Vertex
  {


    public Vector4 Position;

    public bool HasNormal { get; set; }
    public Vector4 Normal;

    public short Index1 { get; set; }
    public short Index2 { get; set; }
    public short Index3 { get; set; }
    public short Index4 { get; set; }
    public short Index5 { get; set; }
    public short Index6 { get; set; }
    public short Index7 { get; set; }
    public short Index8 { get; set; }

    public bool HasWeight1 { get; set; }
    public bool HasWeight2 { get; set; }
    public bool HasWeight3 { get; set; }
    public bool HasWeight4 { get; set; }
    public bool HasWeight5 { get; set; }
    public bool HasWeight6 { get; set; }
    public bool HasWeight7 { get; set; }
    public bool HasWeight8 { get; set; }
    public float Weight1 { get; set; }
    public float Weight2 { get; set; }
    public float Weight3 { get; set; }
    public float Weight4 { get; set; }
    public float Weight5 { get; set; }
    public float Weight6 { get; set; }
    public float Weight7 { get; set; }
    public float Weight8 { get; set; }

    public bool HasSkinningData
    {
      get => HasWeight1;
    }

  }

}
