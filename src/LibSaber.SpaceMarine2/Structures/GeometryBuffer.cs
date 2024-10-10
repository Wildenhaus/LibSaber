using LibSaber.SpaceMarine2.Enumerations;

namespace LibSaber.SpaceMarine2.Structures;

public class GeometryBuffer
{

  #region Properties

  public ushort FlagSize { get; set; }
  public FVFFlags Flags { get; set; }
  public ushort ElementSize { get; set; }
  public uint BufferLength { get; set; }
  public long StartOffset { get; set; }
  public long EndOffset { get; set; }

  public int Count
  {
    get => (int)((EndOffset - StartOffset) / ElementSize);
  }

  public GeometryElementType ElementType
  {
    get => GetElementType();
  }

  #endregion

  #region Private Methods

  private GeometryElementType GetElementType()
  {
    const FVFFlags FLAGS_INTERLEAVED =
      FVFFlags.TANG0 |
      FVFFlags.TANG1 |
      FVFFlags.TANG2 |
      FVFFlags.TANG3 |
      FVFFlags.TANG4 |
      FVFFlags.COLOR0 |
      FVFFlags.COLOR1 |
      FVFFlags.COLOR2 |
      FVFFlags.COLOR3 |
      FVFFlags.COLOR4 |
      FVFFlags.COLOR5 |
      FVFFlags.TEX0 |
      FVFFlags.TEX1 |
      FVFFlags.TEX2 |
      FVFFlags.TEX3 |
      FVFFlags.TEX4 |
      FVFFlags.TEX5;

    if ((Flags & FVFFlags.VERT) != 0)
      return GeometryElementType.Vertex;

    if ((Flags & FLAGS_INTERLEAVED) != 0)
      return GeometryElementType.Interleaved;

    if (Flags == FVFFlags.NONE)
      return GeometryElementType.Face;

    return GeometryElementType.Unknown;
  }

  #endregion

}
