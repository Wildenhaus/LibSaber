using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderGlossiness
  {

    [ScriptingProperty( "bias" )]
    public Single Bias { get; set; }

    [ScriptingProperty( "scale" )]
    public Single Scale { get; set; }

  }

}
