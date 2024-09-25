using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderLayerMultipliers
  {

    [ScriptingProperty( "layerScaleAtten" )]
    public Single LayerScaleAttenuation { get; set; }

    [ScriptingProperty( "layerWaveAtten" )]
    public Single LayerWaveAttenuation { get; set; }

  }

}
