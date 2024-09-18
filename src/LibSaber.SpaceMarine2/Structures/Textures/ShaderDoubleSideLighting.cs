using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderDoubleSideLighting
  {

    [ScriptingProperty( "backLightTint" )]
    public SaberColor BackLightTint { get; set; }

    [ScriptingProperty( "use" )]
    public Boolean Use { get; set; }

    [ScriptingProperty( "viewDependence" )]
    public Single ViewDependence { get; set; }

  }

}
