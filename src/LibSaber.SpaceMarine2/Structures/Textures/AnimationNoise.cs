using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class AnimationNoise
  {

    [ScriptingProperty( "amplitude" )]
    public Single Amplitude { get; set; }

    [ScriptingProperty( "speed" )]
    public Single Speed { get; set; }

  }

}
