using LibSaber.SpaceMarine2.Serialization;
using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class ShaderPerturbationLayer : ITextureNameProvider
  {

    [ScriptingProperty( "NM" )]
    public String NormalMap { get; set; }

    [ScriptingProperty( "scale" )]
    public Single Scale { get; set; }

    [ScriptingProperty( "time_scale" )]
    public Single TimeScale { get; set; }

    [ScriptingProperty( "wave_len" )]
    public Single WaveLength { get; set; }

    public IEnumerable<string> GetTextureNames()
    {
      if ( !string.IsNullOrWhiteSpace( NormalMap ) )
        yield return NormalMap;
    }
  }

}
