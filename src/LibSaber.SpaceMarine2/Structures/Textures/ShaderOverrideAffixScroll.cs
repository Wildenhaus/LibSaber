using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderOverrideAffixScroll
  {

    [ScriptingProperty( "override" )]
    public Boolean OverrideEnabled { get; set; }

    [ScriptingProperty( "speedX" )]
    public Single SpeedX { get; set; }

    [ScriptingProperty( "speedY" )]
    public Single SpeedY { get; set; }

  }

}
