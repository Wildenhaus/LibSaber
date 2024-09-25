using LibSaber.SpaceMarine2.Serialization;
using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderDistortionBackground : ITextureNameProvider
  {

    [ScriptingProperty( "affectedBySelfDistortion" )]
    public Boolean AffectedBySelfDistortion { get; set; }

    [ScriptingProperty( "strength" )]
    public Single Strength { get; set; }

    [ScriptingProperty( "texture" )]
    public String Texture { get; set; }

    [ScriptingProperty( "textureRotation" )]
    public Single TextureRotation { get; set; }

    [ScriptingProperty( "textureScaleX" )]
    public Single TextureScaleX { get; set; }

    [ScriptingProperty( "textureScaleY" )]
    public Single TextureScaleY { get; set; }

    [ScriptingProperty( "textureSpeedScale" )]
    public Single TextureSpeedScale { get; set; }

    public IEnumerable<string> GetTextureNames()
    {
      if ( !string.IsNullOrWhiteSpace( Texture ) )
        yield return Texture;
    }
  }

}
