using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderCarPaint
  {

    [ScriptingProperty( "glossiness" )]
    public ShaderGlossiness Glossiness { get; set; }

    [ScriptingProperty( "metalness" )]
    public SaberColor Metalness { get; set; }

  }

}
