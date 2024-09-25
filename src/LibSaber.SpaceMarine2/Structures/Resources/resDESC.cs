using LibSaber.SpaceMarine2.Enumerations;

namespace LibSaber.SpaceMarine2.Structures.Resources;


public class resDESC
{

  public string __type { get; set; }

}

#region Types

[ResDESCTypeDiscriminator("res_desc_pct")]
public class resDESC_PCT : resDESC
{

  public PctResourceHeader header { get; set; }
  public string linkTd { get; set; }
  public string[] mipMaps { get; set; }
  public string texName { get; set; }
  public string texType { get; set; }
  public string pct { get; set; }

  public SM2TextureFormat TextureFormat => (SM2TextureFormat)header.format;

  public class PctResourceHeader
  {
    public long faceSize { get; set; }
    public int format { get; set; }
    public PctResourceMipLevel[] mipLevel { get; set; }
    public int nFaces { get; set; }
    public int nMipMap { get; set; }
    public long sign { get; set; }
    public long size { get; set; }
    public int sx { get; set; }
    public int sy { get; set; }
    public int sz { get; set; }
  }

  public class PctResourceMipLevel
  {
    public long offset { get; set; }
    public long size { get; set; }
  }

}

[ResDESCTypeDiscriminator("res_desc_scene")]
public class resDESC_SCENE : resDESC
{
  public string cdList { get; set; }
  public string cdt { get; set; }
  public string classList { get; set; }
  public string domList { get; set; }
  public string ecsData { get; set; }
  public string geomDbg { get; set; }
  public string lg { get; set; }
  public string lgData { get; set; }
  public string lightProbesGrid { get; set; }
  public string linkCubemaps { get; set; }
  public string linkMmlCfg { get; set; }
  public string linkReplicas { get; set; }
  public string linkSceneCommon { get; set; }
  public string linkStreamingSequenceReplays { get; set; }
  public string[] links { get; set; }
  public string[] linksMorpheme { get; set; }
  public string[] linksPct { get; set; }
  public string[] linksPreset { get; set; }
  public string[] linksSound { get; set; }
  public string[] linksTd { get; set; }
  public string[] linksTpl { get; set; }
  public string[] linksTplStatic { get; set; }
  public string[] lwiContainers { get; set; }
  public string lwiInstData { get; set; }
  public string nav { get; set; }
  public string obb { get; set; }
  public string pssmData { get; set; }
  public string rain { get; set; }
  public string soundData { get; set; }
  public string ssl { get; set; }
  public string sslData { get; set; }
  public string sslVars { get; set; }
  public string terrain { get; set; }
}

[ResDESCTypeDiscriminator("res_desc_shaders")]
public class resDESC_SHADERS : resDESC
{

  public string[] shaders { get; set; }

}

[ResDESCTypeDiscriminator("res_desc_td")]
public class resDESC_TD : resDESC
{
  public string[] linksPct { get; set; }
  public string materialTemplates { get; set; }
  public string name { get; set; }
  public string[] sdrPresets { get; set; }
  public string td { get; set; }
  public string tdDefaults { get; set; }
}

[ResDESCTypeDiscriminator("res_desc_tpl")]
public class resDESC_TPL : resDESC
{
  public string cdt { get; set; }
  public string gromDbg { get; set; }
  public string[] links { get; set; }
  public string[] linksAnim { get; set; }
  public string[] linksPct { get; set; }
  public string[] linksTd { get; set; }
  public string[] linksTpl { get; set; }
  public string linkTplMarkup { get; set; }
  public string[] tags { get; set; }
  public string tpl { get; set; }
  public bool tplAsset { get; set; }
  public string tplData { get; set; }
  public string[] tplChunks { get; set; }
  public string tplLodsBase { get; set; }
  public string tplMarkup { get; set; }
}

#endregion

#region Attributes

[AttributeUsage(AttributeTargets.Class)]
public class ResDESCTypeDiscriminatorAttribute : Attribute
{

  public string TypeName { get; set; }

  public ResDESCTypeDiscriminatorAttribute(string typeName)
  {
    ASSERT(!string.IsNullOrWhiteSpace(typeName),
      "ResourceDescriptionTypeNameAttribute cannot have an empty value.");

    TypeName = typeName; 
  }

}

#endregion