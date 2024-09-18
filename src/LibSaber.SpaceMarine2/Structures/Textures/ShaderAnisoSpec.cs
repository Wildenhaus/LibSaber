using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderAnisoSpec
  {

    [ScriptingProperty( "angle" )]
    public Single Angle { get; set; }

    [ScriptingProperty( "enabled" )]
    public Boolean Enabled { get; set; }

    [ScriptingProperty( "extra" )]
    public ShaderAnisoSpecExtra Extra { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "tint" )]
    public SaberColor Tint { get; set; }

    [ScriptingProperty("dirMap")]
    public string DirMap { get; set; }

    [ScriptingProperty("anisotropy")]
    public float Anisotropy { get; set; }

    [ScriptingProperty("shift")]
    public float Shift { get; set; }

  }

}
