using LibSaber.SpaceMarine2.Serialization;
using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderEmissive : ITextureNameProvider
  {

    [ScriptingProperty( "adaptiveIntensity" )]
    public Boolean AdaptiveIntensity { get; set; }

    [ScriptingProperty( "animation" )]
    public ShaderAnimation Animation { get; set; }

    [ScriptingProperty( "bloomIntensity" )]
    public Single BloomIntensity { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "tint" )]
    public SaberColor Tint { get; set; }

    public IEnumerable<string> GetTextureNames()
    {
      if ( Animation != null )
        foreach ( var textureName in Animation.GetTextureNames() )
          yield return textureName;
    }
  }

}
