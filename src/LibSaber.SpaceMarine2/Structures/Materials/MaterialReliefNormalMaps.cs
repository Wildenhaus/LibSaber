using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Materials
{

  public class MaterialReliefNormalMaps
  {

    [ScriptingProperty( "macro" )]
    public MaterialNormalMap Macro { get; set; }

    [ScriptingProperty( "micro1" )]
    public MaterialNormalMap Micro1 { get; set; }

    [ScriptingProperty( "micro2" )]
    public MaterialNormalMap Micro2 { get; set; }

  }

}
