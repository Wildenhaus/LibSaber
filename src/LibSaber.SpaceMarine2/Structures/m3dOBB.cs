using System.Numerics;
using LibSaber.IO;

namespace LibSaber.SpaceMarine2.Structures
{

  /// <summary>
  ///   Oriented Bounding Box
  /// </summary>
  public readonly struct m3dOBB
  {

    #region Data Members

    // 0x00: Origin
    public readonly Vector3 org;

    // 0x0C: Unknown (Rotation matrix?)
    public readonly Matrix4x4 Unk_0C;

    // 0x30: Size
    public readonly Vector3 size;

    #endregion

    #region Constructor

    public m3dOBB(Vector3 org, Matrix4x4 unk, Vector3 size)
    {
      this.org = org;
      this.Unk_0C = unk;
      this.size = size;
    }

    #endregion

    internal static m3dOBB Deserialize(NativeReader reader)
    {
      var org = reader.ReadVector3();
      var unk = reader.ReadMatrix3x3();
      var size = reader.ReadVector3();

      return new m3dOBB(org, unk, size);
    }

  }

}
