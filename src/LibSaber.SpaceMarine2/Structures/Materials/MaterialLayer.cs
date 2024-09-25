using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Materials
{

  public class MaterialLayer
  {

    [ScriptingProperty( "texName" )]
    public string TextureName { get; set; }

    [ScriptingProperty( "mtlName" )]
    public string MaterialName { get; set; }

    [ScriptingProperty( "tint" )]
    public float[] Tint { get; set; }

    [ScriptingProperty( "vcSet" )]
    public int VcSet { get; set; }

    [ScriptingProperty( "tilingU" )]
    public float TilingU { get; set; }

    [ScriptingProperty( "tilingV" )]
    public float TilingV { get; set; }

    [ScriptingProperty( "blending" )]
    public MaterialBlending Blending { get; set; }

    [ScriptingProperty( "uvSetIdx" )]
    public int UvSetIndex { get; set; }

  }

}
