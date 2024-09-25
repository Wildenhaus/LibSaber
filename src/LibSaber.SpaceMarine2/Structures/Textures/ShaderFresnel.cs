using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderFresnel
  {

    [ScriptingProperty( "multiplier" )]
    public Single Multiplier { get; set; }

    [ScriptingProperty( "power" )]
    public Single Power { get; set; }

    [ScriptingProperty( "R0" )]
    public Single R0 { get; set; }

    [ScriptingProperty("intensity")]
    public float Intensity { get; set; }

    [ScriptingProperty("tint")]
    public SaberColor Tint { get; set; }

  }

}
