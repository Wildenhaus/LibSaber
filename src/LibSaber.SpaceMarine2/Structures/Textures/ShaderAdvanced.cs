using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderAdvanced
  {

    [ScriptingProperty( "causticsMultiplier" )]
    public Single CausticsMultiplier { get; set; }

    [ScriptingProperty( "fresnel" )]
    public ShaderFresnel Fresnel { get; set; }

    [ScriptingProperty( "kSoft" )]
    public Single KSoft { get; set; }

  }

}
