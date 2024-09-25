using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderFoamWave
  {

    [ScriptingProperty( "attenuation" )]
    public Single Attenuation { get; set; }

    [ScriptingProperty( "height" )]
    public Single Height { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "length" )]
    public Single Length { get; set; }

    [ScriptingProperty( "size" )]
    public Single Size { get; set; }

    [ScriptingProperty( "speed" )]
    public Single Speed { get; set; }

    [ScriptingProperty( "tail" )]
    public Int32 Tail { get; set; }

  }

}
