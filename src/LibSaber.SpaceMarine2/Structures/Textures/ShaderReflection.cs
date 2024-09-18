using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderReflection
  {

    [ScriptingProperty( "contrast" )]
    public Single Contrast { get; set; }

    [ScriptingProperty( "disturbance" )]
    public Single Disturbance { get; set; }

    [ScriptingProperty( "frameSkip" )]
    public Int32 FrameSkip { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "mipPower" )]
    public Single MipPower { get; set; }

    [ScriptingProperty( "useCubeMap" )]
    public Boolean UseCubeMap { get; set; }

    [ScriptingProperty("disableSSR")]
    public bool DisableSSR { get; set; }

  }

}
