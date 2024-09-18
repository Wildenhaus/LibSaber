using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures;

public class ShaderFuzzy
{

  [ScriptingProperty("enabled")]
  public bool Enabled { get; set; }

  [ScriptingProperty("coreDarkness")]
  public float CoreDarkness { get; set; }

  [ScriptingProperty("power")]
  public float Power { get; set; }

  [ScriptingProperty("edgeBrightness")]
  public float EdgeBrightness { get; set; }

  [ScriptingProperty("edgeDesat")]
  public float EdgeDesat { get; set; }

  [ScriptingProperty("edgeColor")]
  public SaberColor EdgeColor { get; set; }

}
