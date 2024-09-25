using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Materials;

public class MaterialExtraUVData
{

  [ScriptingProperty("uvSetIdx")]
  public int UvSetIdx { get; set; }

}
