using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderEyes
  {

    [ScriptingProperty( "glossiness" )]
    public Single Glossiness { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "nmScale" )]
    public Single NormalMapScale { get; set; }

    [ScriptingProperty( "x" )]
    public Single X { get; set; }

    [ScriptingProperty( "y" )]
    public Single Y { get; set; }

  }

}
