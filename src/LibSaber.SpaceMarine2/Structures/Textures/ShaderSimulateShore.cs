using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderSimulateShore
  {

    [ScriptingProperty( "enable" )]
    public Boolean Enable { get; set; }

    [ScriptingProperty( "foam" )]
    public ShaderFoam Foam { get; set; }

    [ScriptingProperty( "level" )]
    public Single Level { get; set; }

    [ScriptingProperty( "wave" )]
    public ShaderFoamWave Wave { get; set; }

  }

}
