namespace LibSaber.SpaceMarine2.Structures;

public class objLOD_ROOT
{

  public List<uint> objIds { get; set; }
  public List<uint> maxObjectLogIndices { get; set; }
  public List<objLOD_DIST> lodDists { get; set; }
  public m3dBOX bbox { get; set; }
  public float SkipFloat { get; set; }
  public bool dontApplyBias { get; set; }

}
