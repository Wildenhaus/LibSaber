using LibSaber.SpaceMarine2.Serialization;
using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class TextureLayer : ITextureNameProvider
  {

    [ScriptingProperty( "alphaSource" )]
    public String AlphaSource { get; set; }

    [ScriptingProperty( "blendMode" )]
    public String BlendMode { get; set; }

    [ScriptingProperty( "speedScale" )]
    public Single SpeedScale { get; set; }

    [ScriptingProperty("speedScaleX")]
    public Single SpeedScaleX { get; set; }

    [ScriptingProperty("speedScaleY")]
    public Single SpeedScaleY { get; set; }

    [ScriptingProperty( "texture" )]
    public String Texture { get; set; }

    [ScriptingProperty( "textureRotation" )]
    public Single TextureRotation { get; set; }

    [ScriptingProperty( "textureScaleX" )]
    public Single TextureScaleX { get; set; }

    [ScriptingProperty( "textureScaleY" )]
    public Single TextureScaleY { get; set; }

    public IEnumerable<string> GetTextureNames()
    {
      if ( !string.IsNullOrWhiteSpace( AlphaSource ) )
        yield return AlphaSource;

      if ( !string.IsNullOrWhiteSpace( Texture ) )
        yield return Texture;
    }
  }

}
