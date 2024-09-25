using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Materials
{

  public class MaterialExtraVertexColorData
  {

    [ScriptingProperty( "colorA" )]
    public MaterialColor ColorA { get; set; }

    [ScriptingProperty( "colorB" )]
    public MaterialColor ColorB { get; set; }

    [ScriptingProperty( "colorG" )]
    public MaterialColor ColorG { get; set; }

    [ScriptingProperty( "colorR" )]
    public MaterialColor ColorR { get; set; }

  }

}
