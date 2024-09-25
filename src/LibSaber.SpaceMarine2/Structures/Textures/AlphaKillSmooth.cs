using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class AlphaKillSmooth
  {

    [ScriptingProperty( "enable" )]
    public Boolean Enable { get; set; }

    [ScriptingProperty( "smoothness" )]
    public Single Smoothness { get; set; }

  }

}
