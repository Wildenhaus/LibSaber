using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderSimulateFlow
  {

    [ScriptingProperty( "disturbance" )]
    public Single Disturbance { get; set; }

    [ScriptingProperty( "enable" )]
    public Boolean Enable { get; set; }

    [ScriptingProperty( "speed" )]
    public Single Speed { get; set; }

  }

}
