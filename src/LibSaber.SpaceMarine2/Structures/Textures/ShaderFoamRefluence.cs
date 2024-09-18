using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderFoamRefluence
  {

    [ScriptingProperty( "amplitude" )]
    public Single Amplitude { get; set; }

    [ScriptingProperty( "freq" )]
    public Single Frequency { get; set; }

    [ScriptingProperty( "posStart" )]
    public Single PositionStart { get; set; }

  }

}
