using System.Numerics;

namespace LibSaber.SpaceMarine2.Structures;

public class LwiContainer : List<LwiContainer.Element>
{

  public uint Version { get; set; }

  public class Element : List<Element.Instance>
  {

    public string Name { get; set; }
    public string Type { get; set; }
    public byte CreateCollisionActor { get; set; }
    public string TplName { get; set; }

    public List<Archetype> Archetypes { get; set; } 

    public uint ElementId { get; set; }
    public uint HashedMaterialOverrides { get; set; }
    public uint Unk_49 { get; set; }
    public long __type { get; set; }

    public class Archetype
    {
      public string TextureName { get; set; }
      public string SourceMaterialName { get; set; }
      public string TargetMaterialName { get; set; }
      public string ObjectName { get; set; }
      public byte Name { get; set; }
    }

    public class Instance
    {
      public Matrix4x4 Mat { get; set; }
      public (ushort,Matrix4x4)[] UnkMatData { get; set; }
      public byte MaterialIndex { get; set; }
      public byte UserInNavmesh { get; set; }
      public byte Unk { get; set; }
    }

  }

}
