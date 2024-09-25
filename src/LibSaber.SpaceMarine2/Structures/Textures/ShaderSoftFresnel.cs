using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderSoftFresnel
  {

    [ScriptingProperty( "edge" )]
    public Boolean Edge { get; set; }

    [ScriptingProperty( "edgeHighlightIntensity" )]
    public Single EdgeHighlightIntensity { get; set; }

    [ScriptingProperty( "enabled" )]
    public Boolean Enabled { get; set; }

    [ScriptingProperty( "power" )]
    public Single Power { get; set; }

  }

}
