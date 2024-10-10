using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Structures;
using LibSaber.SpaceMarine2.Enumerations;

namespace LibSaber.SpaceMarine2.Serialization.Geometry;

public class FVFFlagsSerializer : SM2SerializerBase<FVFFlags>
{

  public override FVFFlags Deserialize(NativeReader reader, ISerializationContext context)
  {
    var bitSet = BitSet<short>.Deserialize(reader, context);
    return MapBitArrayToFVFFlags(bitSet);
  }

  private static FVFFlags MapBitArrayToFVFFlags(BitSet bits)
  {
    FVFFlags flags = FVFFlags.NONE;

    if (bits[0]) flags |= FVFFlags.VERT;
    if (bits[1]) flags |= FVFFlags.VERT_4D;
    if (bits[2]) flags |= FVFFlags.VERT_2D;
    if (bits[3]) flags |= FVFFlags.VERT_COMPR;

    if (bits[7]) flags |= FVFFlags.WEIGHT4;

    if (bits[9]) flags |= FVFFlags.INDICES;
    if (bits[10]) flags |= FVFFlags.NORM;
    if (bits[11]) flags |= FVFFlags.NORM_COMPR;

    if (bits[12]) flags |= FVFFlags.TANG0;
    if (bits[13]) flags |= FVFFlags.TANG1;
    if (bits[14]) flags |= FVFFlags.TANG2;
    if (bits[15]) flags |= FVFFlags.TANG3;
    if (bits[16]) flags |= FVFFlags.TANG4;
    if (bits[17]) flags |= FVFFlags.TANG_COMPR;

    if (bits[22]) flags |= FVFFlags.COLOR0;
    if (bits[23]) flags |= FVFFlags.COLOR1;
    if (bits[24]) flags |= FVFFlags.COLOR2;
    if (bits[25]) flags |= FVFFlags.TEX0;
    if (bits[26]) flags |= FVFFlags.TEX1;
    if (bits[27]) flags |= FVFFlags.TEX2;
    if (bits[28]) flags |= FVFFlags.TEX3;
    if (bits[29]) flags |= FVFFlags.TEX4;
    if (bits[30]) flags |= FVFFlags.TEX0_COMPR;
    if (bits[31]) flags |= FVFFlags.TEX1_COMPR;
    if (bits[32]) flags |= FVFFlags.TEX2_COMPR;
    if (bits[33]) flags |= FVFFlags.TEX3_COMPR;
    if (bits[34]) flags |= FVFFlags.TEX4_COMPR;
    if (bits[35]) flags |= FVFFlags.TEX0_4D;
    if (bits[36]) flags |= FVFFlags.TEX1_4D;
    if (bits[37]) flags |= FVFFlags.TEX2_4D;
    if (bits[38]) flags |= FVFFlags.TEX3_4D;
    if (bits[39]) flags |= FVFFlags.TEX4_4D;
    if (bits[40]) flags |= FVFFlags.TEX0_4D_BYTE;
    if (bits[41]) flags |= FVFFlags.TEX1_4D_BYTE;
    if (bits[42]) flags |= FVFFlags.TEX2_4D_BYTE;
    if (bits[43]) flags |= FVFFlags.TEX3_4D_BYTE;
    if (bits[44]) flags |= FVFFlags.TEX4_4D_BYTE;
    if (bits[45]) flags |= FVFFlags.NORM_IN_VERT4;
    if (bits[46]) flags |= FVFFlags.COLOR3;
    if (bits[47]) flags |= FVFFlags.COLOR4;

    if (bits[59]) flags |= FVFFlags.TEX5;
    if (bits[60]) flags |= FVFFlags.TEX5_COMPR;
    if (bits[61]) flags |= FVFFlags.TEX5_4D;
    if (bits[62]) flags |= FVFFlags.TEX5_4D_BYTE;
    if (bits[63]) flags |= FVFFlags.COLOR5;

    if (bits[67]) flags |= FVFFlags.MASKING_FLAGS;
    if (bits[68]) flags |= FVFFlags.BS_INFO;
    if (bits[69]) flags |= FVFFlags.WEIGHT8;
    if (bits[70]) flags |= FVFFlags.INDICES16;

    return flags;
  }

}
