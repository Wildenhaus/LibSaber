using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures;

public class ShaderFadeMask
{

  [ScriptingProperty("R")]
  public bool R { get; set; }
  [ScriptingProperty("G")]
  public bool G { get; set; }
  [ScriptingProperty("B")]
  public bool B { get; set; }
  [ScriptingProperty("A")]
  public bool A { get; set; }

}
