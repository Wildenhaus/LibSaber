using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures;

public class ShaderTemporalAA
{
  [ScriptingProperty("affectTransparencyMask")]
  public bool AffectTransparencyMask { get; set; }
}
