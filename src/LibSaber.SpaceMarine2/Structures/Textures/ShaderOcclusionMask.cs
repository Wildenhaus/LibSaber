using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures;

public class ShaderOcclusionMask
{

  [ScriptingProperty("vertex_scale")]
  public float VertexScale { get; set; }

  [ScriptingProperty("tex_scale")]
  public float TexScale { get; set; }

}
