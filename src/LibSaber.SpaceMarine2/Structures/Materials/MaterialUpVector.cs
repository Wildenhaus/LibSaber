using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Materials
{

  public class MaterialUpVector
  {

    [ScriptingProperty( "angle" )]
    public float Angle { get; set; }

    [ScriptingProperty( "enabled" )]
    public bool Enabled { get; set; }

    [ScriptingProperty( "falloff" )]
    public float Falloff { get; set; }

  }

}
