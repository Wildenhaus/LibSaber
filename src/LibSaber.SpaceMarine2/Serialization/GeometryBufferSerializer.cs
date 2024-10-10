using LibSaber.IO;
using LibSaber.Serialization;
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
      // The data stored in each buffer is defined by a set of flags.

      buffer.Flags = Serializer<FVFFlags>.Deserialize(reader);
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