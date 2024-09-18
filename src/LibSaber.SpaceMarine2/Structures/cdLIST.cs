using System.Numerics;

namespace LibSaber.SpaceMarine2.Structures;

public class cdLIST : List<cdLIST.ENTRY>
{

  public cdLIST() : base() { }

  public cdLIST(int capacity) : base(capacity) { }

  public class ENTRY
  {
    #region Properties

    public string Name { get; set; }
    public Matrix4x4 Matrix { get; set; }
    public string Affixes { get; set; }
    public string Script { get; set; }
    public Dictionary<string,dynamic> ScriptData { get; set; }

    public string __type { get; set; }
    public List<string> NameTpls { get; set; }

    #endregion
  }

}