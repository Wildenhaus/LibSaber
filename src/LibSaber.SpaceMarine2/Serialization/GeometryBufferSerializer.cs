using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Structures;
using LibSaber.SpaceMarine2.Enumerations;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public sealed class GeometryBufferSerializer : SM2SerializerBase<List<GeometryBuffer>>
{

  #region Overrides

  public override List<GeometryBuffer> Deserialize(NativeReader reader, ISerializationContext context)
  {
    var graph = context.GetMostRecentObject<objGEOM_MNG>();
    var sectionEndOffset = reader.ReadInt32();

    var count = graph.BufferCount;
    var buffers = new List<GeometryBuffer>(count);
    for (var i = 0; i < count; i++)
      buffers.Add(new GeometryBuffer());

    var sentinelReader = new SentinelReader<int>(reader);
    while (reader.Position < sectionEndOffset)
    {
      sentinelReader.Next();
      switch (sentinelReader.SentinelId)
      {
        case GeometryBufferSentinels.Flags: //fvf
          ReadBufferFlags(reader, buffers);
          break;
        case GeometryBufferSentinels.ElementSizes: //stride
          ReadBufferElementSizes(reader, buffers);
          break;
        case GeometryBufferSentinels.Lengths: //size
          ReadBufferLengths(reader, buffers);
          break;
        case GeometryBufferSentinels.Data:
          ReadBufferData(reader, buffers, context);
          break;
        case 4:
          // OBJ_GEOM_STREAM_FLAGS_F
          reader.Position = sentinelReader.EndOffset;
          break;

        default:
          sentinelReader.ReportUnknownSentinel();
          break;
      }
    }

    ASSERT(reader.Position == sectionEndOffset,
        "Reader position does not match the buffer section's end offset.");

    return buffers;
  }

  #endregion

  #region Private Methods

  private void ReadBufferFlags(NativeReader reader, List<GeometryBuffer> geometryBuffers)
  {
    foreach (var buffer in geometryBuffers)
    {
      /* The data stored in each buffer is defined by a set of flags.
       * This is usually 0x3B or 0x3F in length.
       * It appears that it's always stored as a 64-bit int.
       */

      var set = BitSet<short>.Deserialize(reader, null);
      buffer.Flags = set.As<GeometryBufferFlags>();
      buffer.FlagSet = MapBitArrayToFVFFlags(set);
    }
  }

  private void ReadBufferElementSizes(NativeReader reader, List<GeometryBuffer> geometryBuffers)
  {
    /* The size of each element in the buffer (stride length).
     * We can use this to calculate offsets and catch cases where we under/overread each element.
     */

    foreach (var buffer in geometryBuffers)
      buffer.ElementSize = reader.ReadUInt16();
  }

  private void ReadBufferLengths(NativeReader reader, List<GeometryBuffer> geometryBuffers)
  {
    /* The total length (in bytes) of the buffer.
     */

    foreach (var buffer in geometryBuffers)
      buffer.BufferLength = reader.ReadUInt32();

    long startOffset = 0;
    foreach (var buffer in geometryBuffers)
    {
      buffer.StartOffset = startOffset;
      buffer.EndOffset = startOffset + buffer.BufferLength;
      startOffset = buffer.EndOffset;
    }
  }

  private void ReadBufferData(NativeReader reader, List<GeometryBuffer> geometryBuffers, ISerializationContext context)
  {
    /* This is the actual buffer data. We're using the previously obtained length
     * to determine the start and end offsets.
     * 
     * We can also optionally deserialize the buffer elements here.
     */

    var graph = context.GetMostRecentObject<objGEOM_MNG>();
    graph.ContainsVertexBuffers = true;

    foreach (var buffer in geometryBuffers)
    {
      buffer.StartOffset = reader.Position;
      buffer.EndOffset = buffer.StartOffset + buffer.BufferLength;

      reader.Position = buffer.EndOffset;
    }
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

  #endregion

  #region Sentinel Ids

  private class GeometryBufferSentinels
  {
    public const short Flags = 0x0000;
    public const short ElementSizes = 0x0001;
    public const short Lengths = 0x0002;
    public const short Data = 0x0003;
  }

  #endregion

}