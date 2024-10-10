using System.Numerics;
using LibSaber.Extensions;
using LibSaber.SpaceMarine2.Enumerations;
using LibSaber.SpaceMarine2.Structures;
using LibSaber.SpaceMarine2.Structures.Geometry;
using LibSaber.IO;

namespace LibSaber.SpaceMarine2.Serialization.Geometry
{

  public sealed class InterleavedDataSerializer : GeometryElementSerializer<InterleavedData>
  {

    #region Constructor

    public InterleavedDataSerializer( GeometryBuffer buffer )
      : base( buffer )
    {
    }

    #endregion

    #region Overrides

    public override InterleavedData Deserialize( NativeReader reader )
    {
      var startPos = reader.Position;
      var endPos = reader.Position + Buffer.ElementSize;

      var data = new InterleavedData();

      ReadTangents(reader, ref data);
      ReadVertexColors(reader, ref data);
      ReadBsInfo(reader);
      ReadUVs(reader, ref data);

      var readSize = reader.Position - startPos;
      ASSERT(Buffer.ElementSize == readSize);

      return data;
    }

    public override IEnumerable<InterleavedData> DeserializeRange( NativeReader reader, int startIndex, int endIndex )
    {
      var startOffset = Buffer.StartOffset + ( startIndex * Buffer.ElementSize );
      var length = endIndex - startIndex;

      reader.Position = startOffset;

      for ( var i = 0; i < length; i++ )
        yield return Deserialize( reader );
    }

    #endregion

    #region Private Methods
    #region Tangents

    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
    private void ReadTangents(NativeReader reader, ref InterleavedData data)
    {
      data.HasTangent0 = (Flags & FVFFlags.TANG0) != 0;
      data.HasTangent1 = (Flags & FVFFlags.TANG1) != 0;
      data.HasTangent2 = (Flags & FVFFlags.TANG2) != 0;
      data.HasTangent3 = (Flags & FVFFlags.TANG3) != 0;
      data.HasTangent4 = (Flags & FVFFlags.TANG4) != 0;

      if ((Flags & FVFFlags.TANG_COMPR) != 0)
      {
        if (data.HasTangent0) data.Tangent0 = ReadCompressedTangent(reader);
        if (data.HasTangent1) data.Tangent1 = ReadCompressedTangent(reader);
        if (data.HasTangent2) data.Tangent2 = ReadCompressedTangent(reader);
        if (data.HasTangent3) data.Tangent3 = ReadCompressedTangent(reader);
        if (data.HasTangent4) data.Tangent4 = ReadCompressedTangent(reader);
      }
      else
      {
        if (data.HasTangent0) data.Tangent0 = ReadUncompressedTangent(reader);
        if (data.HasTangent1) data.Tangent1 = ReadUncompressedTangent(reader);
        if (data.HasTangent2) data.Tangent2 = ReadUncompressedTangent(reader);
        if (data.HasTangent3) data.Tangent3 = ReadUncompressedTangent(reader);
        if (data.HasTangent4) data.Tangent4 = ReadUncompressedTangent(reader);
      }
    }

    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
    private Vector4 ReadCompressedTangent(NativeReader reader)
    {
      var x = reader.ReadSByte().SNormToFloat();
      var y = reader.ReadSByte().SNormToFloat();
      var z = reader.ReadSByte().SNormToFloat();
      var w = reader.ReadSByte().SNormToFloat();

      ASSERT(x >= -1.01 && x <= 1.01, "Tangent X coord out of bounds.");
      ASSERT(y >= -1.01 && y <= 1.01, "Tangent Y coord out of bounds.");
      ASSERT(z >= -1.01 && z <= 1.01, "Tangent Z coord out of bounds.");
      ASSERT(w >= -1.01 && w <= 1.01, "Tangent W coord out of bounds.");

      return new Vector4(x, y, z, w);
    }

    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
    private Vector4 ReadUncompressedTangent(NativeReader reader)
    {
      var x = reader.ReadFloat32();
      var y = reader.ReadFloat32();
      var z = reader.ReadFloat32();
      var w = reader.ReadFloat32();

      return new Vector4(x, y, z, w);
    }

    #endregion

    #region Colors

    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
    private void ReadVertexColors(NativeReader reader, ref InterleavedData data)
    {
      data.HasColor0 = (Flags & FVFFlags.COLOR0) != 0;
      data.HasColor1 = (Flags & FVFFlags.COLOR1) != 0;
      data.HasColor2 = (Flags & FVFFlags.COLOR2) != 0;
      data.HasColor3 = (Flags & FVFFlags.COLOR3) != 0;
      data.HasColor4 = (Flags & FVFFlags.COLOR4) != 0;
      data.HasColor5 = (Flags & FVFFlags.COLOR5) != 0;

      if (data.HasColor0) data.Color0 = ReadVertexColor(reader);
      if (data.HasColor1) data.Color1 = ReadVertexColor(reader);
      if (data.HasColor2) data.Color2 = ReadVertexColor(reader);
      if (data.HasColor3) data.Color3 = ReadVertexColor(reader);
      if (data.HasColor4) data.Color4 = ReadVertexColor(reader);
      if (data.HasColor5) data.Color5 = ReadVertexColor(reader);
    }

    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
    private Vector4 ReadVertexColor(NativeReader reader)
    {
      var r = reader.ReadByte().UNormToFloat();
      var g = reader.ReadByte().UNormToFloat();
      var b = reader.ReadByte().UNormToFloat();
      var a = reader.ReadByte().UNormToFloat();

      return new Vector4(r, g, b, a);
    }

    #endregion

    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
    private void ReadBsInfo(NativeReader reader)
    {
      if ((Flags & FVFFlags.BS_INFO) != 0)
        _ = reader.ReadInt16();
    }

    #region UVs

    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
    private void ReadUVs(NativeReader reader, ref InterleavedData data)
    {
      if ((Flags & FVFFlags.TEX0) != 0)
      {
        data.HasUV0 = true;
        data.UV0 = ReadUV(reader, (Flags & FVFFlags.TEX0_COMPR) != 0);
        ASSERT((Flags & FVFFlags.TEX0_4D) == 0, "4D UV");
      }
      if ((Flags & FVFFlags.TEX1) != 0)
      {
        data.HasUV1 = true;
        data.UV1 = ReadUV(reader, (Flags & FVFFlags.TEX1_COMPR) != 0);
        ASSERT((Flags & FVFFlags.TEX1_4D) == 0, "4D UV");
      }
      if ((Flags & FVFFlags.TEX2) != 0)
      {
        data.HasUV2 = true;
        data.UV2 = ReadUV(reader, (Flags & FVFFlags.TEX2_COMPR) != 0);
        ASSERT((Flags & FVFFlags.TEX2_4D) == 0, "4D UV");
      }
      if ((Flags & FVFFlags.TEX3) != 0)
      {
        data.HasUV3 = true;
        data.UV3 = ReadUV(reader, (Flags & FVFFlags.TEX3_COMPR) != 0);
        ASSERT((Flags & FVFFlags.TEX3_4D) == 0, "4D UV");
      }
      if ((Flags & FVFFlags.TEX4) != 0)
      {
        data.HasUV4 = true;
        data.UV4 = ReadUV(reader, (Flags & FVFFlags.TEX4_COMPR) != 0);
        ASSERT((Flags & FVFFlags.TEX4_4D) == 0, "4D UV");
      }
      if ((Flags & FVFFlags.TEX5) != 0)
      {
        data.HasUV5 = true;
        data.UV5 = ReadUV(reader, (Flags & FVFFlags.TEX5_COMPR) != 0);
        ASSERT((Flags & FVFFlags.TEX5_4D) == 0, "4D UV");
      }
    }

    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
    private Vector4 ReadUV(NativeReader reader, bool isCompressed)
    {
      if (isCompressed)
      {
        // Fairly sure this is correct
        var u = reader.ReadInt16().SNormToFloat();
        var v = 1 - reader.ReadInt16().SNormToFloat();

        return new Vector4(u, v, 0, 0);
      }
      else
      {
        // Fairly sure this is correct
        var u = reader.ReadFloat32();
        var v = 1 - reader.ReadFloat32();

        return new Vector4(u, v, 0, 0);
      }
    }

    #endregion

    #endregion

  }

}
