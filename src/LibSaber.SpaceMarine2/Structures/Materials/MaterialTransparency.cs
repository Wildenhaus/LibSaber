using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Materials
{

  public class MaterialTransparency
  {

    [ScriptingProperty( "colorSetIdx" )]
    public int ColorSetIndex { get; set; }

    [ScriptingProperty( "enabled" )]
    public int Enabled { get; set; }

    [ScriptingProperty("dirtModifyAlpha")]
    public int DirtModifyAlpha { get; set; }

    [ScriptingProperty("useBlendMaskAlpha")]
    public int UseBlendMaskAlpha { get; set; }

    [ScriptingProperty( "multiplier" )]
    public float Multiplier { get; set; }

    [ScriptingProperty( "sources" )]
    public int Sources { get; set; }

  }

}
