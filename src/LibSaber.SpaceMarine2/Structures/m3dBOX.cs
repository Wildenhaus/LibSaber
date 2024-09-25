using System.Numerics;

namespace LibSaber.SpaceMarine2.Structures;

public readonly struct m3dBOX
{
  #region Data Members

  public readonly Vector3 Min;
  public readonly Vector3 Max;

  #endregion

  #region Constructor

  public m3dBOX(Vector3 min, Vector3 max)
  {
    Min = min;
    Max = max;
  }

  #endregion
}
