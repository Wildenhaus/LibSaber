using System.Numerics;

namespace LibSaber.SpaceMarine2.Structures;

public class animOBJ_ANIM
{

  public Vector3 iniTranslation { get; set; }
  public m3dSPL pTranslation { get; set; }
  public Vector4 iniRotation { get; set; }
  public m3dSPL pRotation { get; set; }
  public Vector3 iniScale { get; set; }
  public m3dSPL pScale { get; set; }
  public float iniVisibility { get; set; }
  public m3dSPL pVisibility { get; set; }

}
