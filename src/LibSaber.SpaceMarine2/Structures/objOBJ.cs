using System.Numerics;
using LibSaber.Shared.Structures;
using LibSaber.SpaceMarine2.Enumerations;

namespace LibSaber.SpaceMarine2.Structures;

public class objOBJ
{

  #region Data Members

  private string _name;
  private string _boneName;
  private string _meshName;

  #endregion

  #region Properties

  public objGEOM_MNG GeometryGraph { get; }

  public short id { get; set; }
  public string ReadName { get; set; }
  public ObjectStateFlags state { get; set; }

  public short parentId { get; set; }
  public short nextId { get; set; }
  public short prevId { get; set; }
  public short childId { get; set; }

  public short animNmb { get; set; }
  public string SourceId { get; set; }
  public string ReadAffixes { get; set; }

  public Matrix4x4 MatrixLT { get; set; }
  public Matrix4x4 MatrixModel { get; set; }

  public objGEOM_UNSHARED geomData { get; set; }
  public string ps { get; set; }

  public byte objProps { get; set; }
  public string UnkName { get; set; }
  public string Obb { get; set; }
  public string ReadOnlyName { get; set; }
  public string ReadOnlyAffixes { get; set; }
  public string ReadOnlyObjectName { get; set; }
  public string Affixes { get; set; }

  public objOBJ Parent
  {
    get
    {
      if (parentId < 0)
        return null;

      return GeometryGraph.objects[parentId];
    }
  }

  public IEnumerable<GeometrySubMesh> SubMeshes
  {
    get => GeometryGraph.SubMeshes.Where(x => x.NodeId == id);
  }

  #endregion

  #region Constructor

  public objOBJ(objGEOM_MNG geometryGraph)
  {
    GeometryGraph = geometryGraph;
  }

  #endregion

  #region Public Methods

  public IEnumerable<objOBJ> EnumerateChildren()
  {
    var visited = new HashSet<short>();

    var currentId = childId;
    while (visited.Add(currentId))
    {
      if (currentId < 0)
        break;

      yield return GeometryGraph.objects[currentId];
      currentId = GeometryGraph.objects[currentId].nextId;
    }
  }

  public string GetName()
  {
    if (_name != null)
      return _name;

    if (!string.IsNullOrWhiteSpace(UnkName))
      return _name = UnkName.Split(new[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries).Last();

    if (!string.IsNullOrWhiteSpace(ReadOnlyObjectName))
      return _name = ReadOnlyObjectName;

    if (!string.IsNullOrWhiteSpace(ReadName))
      return _name = ReadName;

    return _name = $"Object{id:X4}";
  }

  public string GetParentName()
    => Parent?.GetName();

  public string GetMeshName()
  {
    if (_meshName != null)
      return _meshName;

    if (string.IsNullOrWhiteSpace(UnkName))
      return null;

    var nameParts = UnkName.Split(new[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries)
      .Where(x => x != "h" && !x.StartsWith("_b_") && !x.StartsWith("_m_"));

    return _meshName = string.Join("_", nameParts.TakeLast(2)).Trim();
  }

  public string GetParentMeshName()
  {

    if (string.IsNullOrWhiteSpace(UnkName))
      return null;

    var nameParts = UnkName.Split(new[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries)
      .Where(x => x != "h" && !x.StartsWith("_b_") && !x.StartsWith("_m_"));

    return nameParts.FirstOrDefault();
  }

  public string GetBoneName()
  {
    if (_boneName != null)
      return _boneName;

    if (string.IsNullOrWhiteSpace(UnkName))
      return null;

    var nameParts = UnkName.Split(new[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries)
      .Where(x => x.StartsWith("_b_"));

    return _boneName = nameParts.LastOrDefault();
  }

  #endregion

}
