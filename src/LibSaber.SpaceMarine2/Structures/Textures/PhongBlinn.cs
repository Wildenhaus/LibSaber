using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{
    public class PhongBlinn
  {

    [ScriptingProperty( "multiplier" )]
    public Single Multiplier { get; set; }

    [ScriptingProperty( "normalScale" )]
    public Single NormalScale { get; set; }

    [ScriptingProperty( "power" )]
    public Single Power { get; set; }

    [ScriptingProperty( "spotDepth" )]
    public Single SpotDepth { get; set; }

  }

}
