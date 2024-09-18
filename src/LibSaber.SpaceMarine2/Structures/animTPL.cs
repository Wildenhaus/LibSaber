using LibSaber.SpaceMarine2.Enumerations;

namespace LibSaber.SpaceMarine2.Structures;

public class animTPL
{

  public string name { get; set; }
  public string nameClass { get; set; }
  public TPLState state { get; set; }
  public string affixes { get; set; }
  public string ps { get; set; }
  public tplSKIN skin { get; set; }
  public animTRACK trackAnim { get; set; }
  public m3dBOX bbox { get; set; }
  public List<tplLOD_DEF> lodDef { get; set; }
  public List<string> TexList { get; set; }
  public objGEOM_MNG GeometryGraph { get; set; }
  public string externData { get; set; }

  public bool ContainsVertexBuffers => GeometryGraph?.ContainsVertexBuffers ?? false;

}
