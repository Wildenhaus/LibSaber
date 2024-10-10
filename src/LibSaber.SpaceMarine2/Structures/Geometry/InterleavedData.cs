using System.Numerics;

namespace LibSaber.SpaceMarine2.Structures.Geometry
{

  public struct InterleavedData
  {

    #region Data Members

    public bool HasTangent0;
    public bool HasTangent1;
    public bool HasTangent2;
    public bool HasTangent3;
    public bool HasTangent4;

    public Vector4 Tangent0;
    public Vector4 Tangent1;
    public Vector4 Tangent2;
    public Vector4 Tangent3;
    public Vector4 Tangent4;

    public bool HasColor0;
    public bool HasColor1;
    public bool HasColor2;
    public bool HasColor3;
    public bool HasColor4;
    public bool HasColor5;

    public Vector4 Color0;
    public Vector4 Color1;
    public Vector4 Color2;
    public Vector4 Color3;
    public Vector4 Color4;
    public Vector4 Color5;

    public bool HasUV0;
    public bool HasUV1;
    public bool HasUV2;
    public bool HasUV3;
    public bool HasUV4;
    public bool HasUV5;

    public Vector4 UV0;
    public Vector4 UV1;
    public Vector4 UV2;
    public Vector4 UV3;
    public Vector4 UV4;
    public Vector4 UV5;

    #endregion

  }

}
