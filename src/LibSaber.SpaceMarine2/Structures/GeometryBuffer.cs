﻿using LibSaber.Shared.Structures;
using LibSaber.SpaceMarine2.Enumerations;

namespace LibSaber.SpaceMarine2.Structures;

public class GeometryBuffer
{

  #region Properties

  public ushort FlagSize { get; set; }
  public GeometryBufferFlags Flags { get; set; }
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
    if (Flags.HasFlag(GeometryBufferFlags._VERT))
      return GeometryElementType.Vertex;

    if (Flags.HasFlag(GeometryBufferFlags._TANG0)
      || Flags.HasFlag(GeometryBufferFlags._TANG1)
      || Flags.HasFlag(GeometryBufferFlags._TANG2)
      || Flags.HasFlag(GeometryBufferFlags._TANG3)
      || Flags.HasFlag(GeometryBufferFlags._TEX0)
      || Flags.HasFlag(GeometryBufferFlags._TEX1)
      || Flags.HasFlag(GeometryBufferFlags._TEX2)
      || Flags.HasFlag(GeometryBufferFlags._TEX3)
      || Flags.HasFlag(GeometryBufferFlags._TEX4)
      )
      return GeometryElementType.Interleaved;

    if (Flags == GeometryBufferFlags._FACE)
      return GeometryElementType.Face;

    if (Flags.HasFlag(GeometryBufferFlags._WEIGHT8WithBones))
      return GeometryElementType.BoneId;

    return GeometryElementType.Unknown;
  }

  #endregion

}
