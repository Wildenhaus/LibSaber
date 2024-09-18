using System.Numerics;

namespace LibSaber.SpaceMarine2.Structures;

public class tplSKIN
{
  public uint nBones { get; set; }
  public Matrix4x4[] boneInvBindMatrList { get; set; }
  public ushort[] lodBonesCount { get; set; }
}
