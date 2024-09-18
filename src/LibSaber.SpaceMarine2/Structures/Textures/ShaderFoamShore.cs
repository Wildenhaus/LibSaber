using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderFoamShore
  {

    [ScriptingProperty( "distribuance" )]
    public ShaderFoamDistribuance Distribuance { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "pos0" )]
    public Single Position0 { get; set; }

    [ScriptingProperty( "pos1" )]
    public Single Position1 { get; set; }

    [ScriptingProperty( "pos2" )]
    public Single Position2 { get; set; }

    [ScriptingProperty( "pos3" )]
    public Single Position3 { get; set; }

    [ScriptingProperty( "size" )]
    public Single Size { get; set; }

    [ScriptingProperty( "tile" )]
    public Single Tile { get; set; }

  }

}
