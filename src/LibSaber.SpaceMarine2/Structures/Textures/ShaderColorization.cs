using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures;

public class ShaderColorization
{

  [ScriptingProperty("color")]
  public SaberColor Color { get; set; }

  [ScriptingProperty("use")]
  public bool Use { get; set; }

  [ScriptingProperty("useTranslucencyAsMask")]
  public bool UseTranslucencyAsMask { get; set; }

  [ScriptingProperty("amount")]
  public float Amount { get; set; }

}
