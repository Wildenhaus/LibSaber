﻿using LibSaber.SpaceMarine2.Serialization.Scripting;

namespace LibSaber.SpaceMarine2.Structures.Materials
{

  public class MaterialBlending
  {

    [ScriptingProperty( "method" )]
    public string Method { get; set; }

    [ScriptingProperty( "useLayerAlpha" )]
    public bool UseLayerAlpha { get; set; }

    [ScriptingProperty( "useHeightmap" )]
    public bool UseHeightMap { get; set; }

    [ScriptingProperty("blendNormalAdd")]
    public bool BlendNormalAdd { get; set; }

    [ScriptingProperty( "weightMultiplier" )]
    public float WeightMultiplier { get; set; }

    [ScriptingProperty( "heightmapSoftness" )]
    public float HeightMapSoftness { get; set; }

    [ScriptingProperty( "texChannelBlendMask" )]
    public int TexChannelBlendMask { get; set; }

    [ScriptingProperty( "weights" )]
    public MaterialWeights Weights { get; set; }

    [ScriptingProperty( "heightmap" )]
    public MaterialHeightMap HeightMap { get; set; }

    [ScriptingProperty( "heightmapOverride" )]
    public string HeightMapOverride { get; set; }

    [ScriptingProperty( "upVector" )]
    public MaterialUpVector UpVector { get; set; }

    [ScriptingProperty( "heightmapUVOverride" )]
    public MaterialHeightMapUvOverride HeightMapUvOverride { get; set; }

  }

}
