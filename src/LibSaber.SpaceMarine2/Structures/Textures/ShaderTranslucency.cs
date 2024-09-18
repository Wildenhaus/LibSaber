using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderTranslucency
  {

    [ScriptingProperty( "blurWidth" )]
    public Single BlurWidth { get; set; }

    [ScriptingProperty( "firstColor" )]
    public SaberColor FirstColor { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "secondColor" )]
    public SaberColor SecondColor { get; set; }

    [ScriptingProperty( "width" )]
    public Single Width { get; set; }

  }

}
