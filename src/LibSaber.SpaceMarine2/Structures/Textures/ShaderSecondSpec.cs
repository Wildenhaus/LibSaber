using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures;

public class ShaderSecondSpec
{

  [ScriptingProperty("enabled")]
  public bool Enabled { get; set; }

  [ScriptingProperty("metallicTint")]
  public SaberColor MetallicTint { get; set; }

  [ScriptingProperty("metalness")]
  public ShaderGlossiness Metalness { get; set; }

  [ScriptingProperty("roughness")]
  public ShaderGlossiness Roughness { get; set; }

}
