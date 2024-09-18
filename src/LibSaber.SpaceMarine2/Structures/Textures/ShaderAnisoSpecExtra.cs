using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderAnisoSpecExtra
  {

    [ScriptingProperty("anisotropy")]
    public Single Anisotropy { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "shift" )]
    public Single Shift { get; set; }

  }

}
