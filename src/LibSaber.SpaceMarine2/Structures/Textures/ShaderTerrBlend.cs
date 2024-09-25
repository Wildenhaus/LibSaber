using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures;

public class ShaderTerrBlend
{

  [ScriptingProperty("disable_terr_soft_blend")]
  public bool DisableTerrSoftBlend { get; set; }

  [ScriptingProperty("blend_by_mask")]
  public BlendByMask BlendByMask { get; set; }

  [ScriptingProperty("apply_colorize")]
  public ApplyColorize ApplyColorize { get; set; }

  [ScriptingProperty("distance_blend")]
  public DistanceBlend DistanceBlend { get; set; }

}

public class ApplyColorize
{
  [ScriptingProperty("enabled")]
  public bool Enabled { get; set; }

  [ScriptingProperty("rand_colorize")]
  public bool RandColorize { get; set; }

  [ScriptingProperty("power")]
  public float Power { get; set; }
}

public class BlendByMask
{
  [ScriptingProperty("mask")]
  public string Mask { get; set; }

  [ScriptingProperty("no_save_local_height")]
  public bool NoSaveLocalHeight { get; set; }
}

public class DistanceBlend
{
  [ScriptingProperty("normals_spec_by_dist")]
  public bool NormalsSpecByDist { get; set; }

  [ScriptingProperty("start_dist")]
  public float StartDist { get; set; }

  [ScriptingProperty("power")]
  public float Power { get; set; }
}