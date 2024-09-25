using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class TextureDecay
  {

    [ScriptingProperty( "akill_falloff" )]
    public Single AlphaKillFalloff { get; set; }

    [ScriptingProperty( "akill_minval" )]
    public Single AlphaKillMinValue { get; set; }

    [ScriptingProperty( "akill_start_decay" )]
    public Single AlphaKillStartDecay { get; set; }

    [ScriptingProperty( "use_decay" )]
    public Boolean UseDecay { get; set; }

  }

}
