using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderParallax
  {

    [ScriptingProperty( "scale" )]
    public Single Scale { get; set; }

    [ScriptingProperty( "shadow" )]
    public Boolean Shadow { get; set; }

  }

}
