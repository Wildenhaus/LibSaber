using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures;

public class TextureUvParams
{

  [ScriptingProperty("speedScaleX")]
  public float SpeedScaleX { get; set; }

  [ScriptingProperty("speedScaleY")]
  public float SpeedScaleY { get; set; }

  [ScriptingProperty("texScaleX")]
  public float TexScaleX { get; set; }

  [ScriptingProperty("texScaleY")]
  public float TexScaleY { get; set; }

}
