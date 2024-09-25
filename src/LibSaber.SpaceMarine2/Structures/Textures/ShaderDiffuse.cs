using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderDiffuse
  {

    [ScriptingProperty( "multiplier" )]
    public Single Multiplier { get; set; }

    [ScriptingProperty( "shadowAmbient" )]
    public Single ShadowAmbient { get; set; }

  }

}
