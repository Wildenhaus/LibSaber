using LibSaber.SpaceMarine2.Serialization;
using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderGradient : ITextureNameProvider
  {

    [ScriptingProperty( "speed" )]
    public Single Speed { get; set; }

    [ScriptingProperty( "tex" )]
    public String Texture { get; set; }

    [ScriptingProperty( "texPhaseOffset" )]
    public String TexturePhaseOffset { get; set; }

    public IEnumerable<string> GetTextureNames()
    {
      if ( !string.IsNullOrWhiteSpace( Texture ) )
        yield return Texture;
    }
  }

}
