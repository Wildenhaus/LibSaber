using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSaber.SpaceMarine2.Serialization;
using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures;

public class TextureVertexPulsation : ITextureNameProvider
{

  [ScriptingProperty("enable")]
  public Boolean Enable { get; set; }

  [ScriptingProperty("source_texture")]
  public string SourceTexture { get; set; }

  [ScriptingProperty("texture_uv_params")]
  public TextureUvParams TextureUvParams { get; set; }

  [ScriptingProperty("bending")]
  public ShaderAnimationBending Bending { get; set; }

  [ScriptingProperty("noise")]
  public ShaderAnimationBending Noise { get; set; }

  public IEnumerable<string> GetTextureNames()
  {
    if (SourceTexture != null)
      yield return SourceTexture;
  }

}
