using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderColors
  {

    [ScriptingProperty( "ambientLightOverride" )]
    public SaberColor AmbientLightOverride { get; set; }

    [ScriptingProperty( "deepDepth" )]
    public Single DeepDepth { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "invertedNormal" )]
    public Boolean InvertedNormal { get; set; }

    [ScriptingProperty( "posShallow" )]
    public Single PosShallow { get; set; }

    [ScriptingProperty( "reflectionColorOverride" )]
    public SaberColor ReflectionColorOverride { get; set; }

    [ScriptingProperty( "skyAOOverride" )]
    public Single SkyAoOverride { get; set; }

    [ScriptingProperty( "ssrOverride" )]
    public Single SsrOverride { get; set; }

    [ScriptingProperty( "waterDeep" )]
    public Int32[] WaterDeep { get; set; }

    [ScriptingProperty( "waterDeepIntensity" )]
    public Single WaterDeepIntensity { get; set; }

    [ScriptingProperty( "waterScatter" )]
    public Int32[] WaterScatter { get; set; }

    [ScriptingProperty( "waterShallow" )]
    public Int32[] WaterShallow { get; set; }

  }

}
