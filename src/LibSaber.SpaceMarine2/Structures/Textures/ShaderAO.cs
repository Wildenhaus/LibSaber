using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderAO
  {

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "occlusionAmount" )]
    public Single OcclusionAmount { get; set; }

    [ScriptingProperty( "vertexAmbientOcclusion" )]
    public Boolean VertexAmbientOcclusion { get; set; }

  }

}
