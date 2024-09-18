using System.Collections;

namespace LibSaber.SpaceMarine2.Structures;

public class scnSCENE
{

  #region Properties

  public uint PropertyCount { get; set; }
  public BitArray PropertyFlags { get; set; }

  public List<string> TextureList { get; set; }
  public string PS { get; set; }
  public List<string> InstMaterialInfoList { get; set; }

  public objGEOM_MNG GeometryGraph { get; set; }

  #endregion

}
