using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Textures
{

  public class TextureConvertSettings
  {

    [ScriptingProperty( "color" )]
    public SaberColor Color { get; set; }

    [ScriptingProperty( "fade_begin" )]
    public Int32 FadeBegin { get; set; }

    [ScriptingProperty( "fade_end" )]
    public Int32 FadeEnd { get; set; }

    [ScriptingProperty( "fade_flag" )]
    public Boolean FadeFlag { get; set; }

    [ScriptingProperty( "filter" )]
    public Int32 Filter { get; set; }

    [ScriptingProperty( "format_descr" )]
    public String FormatDescription { get; set; }

    [ScriptingProperty( "format_name" )]
    public String FormatName { get; set; }

    [ScriptingProperty( "fp16" )]
    public Boolean FP16 { get; set; }

    [ScriptingProperty( "left_handed" )]
    public Boolean LeftHanded { get; set; }

    [ScriptingProperty( "mipmap_level" )]
    public Int32 MipMapLevel { get; set; }

    [ScriptingProperty( "resample" )]
    public Int32 Resample { get; set; }

    [ScriptingProperty( "sharpen" )]
    public Int32 Sharpen { get; set; }

    [ScriptingProperty( "sharpness" )]
    public Boolean Sharpness { get; set; }

    [ScriptingProperty( "uncompressed_flag" )]
    public Boolean UncompressedFlag { get; set; }

    [ScriptingProperty( "force_use_oxt1_flag")]
    public Boolean ForceUseOXT1Flag { get; set; }

    [ScriptingProperty( "compressionQuality" )]
    public Int32 CompressionQuality { get; set; }

    [ScriptingProperty("m_akill_ref")]
    public Int32 M_Akill_Ref { get; set; }

    [ScriptingProperty("m_akill_thick")]
    public Int32 M_Akill_Thick { get; set; }

    [ScriptingProperty("fade_mask")]
    public ShaderFadeMask FadeMask { get; set; }

    [ScriptingProperty("fade_rough_flag")]
    public bool FadeRoughFlag { get; set; }

    [ScriptingProperty("fade_rough_mip_bot")]
    public float FadeRoughMipBot { get; set; }

    [ScriptingProperty("fade_rough_mip_top")]
    public float FadeRoughMipTop { get; set; }

    [ScriptingProperty("fade_rough_top_min")]
    public float FadeRoughTopMin { get; set; }

    [ScriptingProperty("auto_mipmap_brightness_param")]
    public int AutoMipBrightnessParam { get; set; }

  }

}
