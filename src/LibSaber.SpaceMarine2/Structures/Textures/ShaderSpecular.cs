using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderSpecular
  {

    [ScriptingProperty( "blinn" )]
    public PhongBlinn Blinn { get; set; }

    [ScriptingProperty( "phong" )]
    public PhongBlinn Phong { get; set; }

    [ScriptingProperty( "tint" )]
    public SaberColor Tint { get; set; }

  }

}
