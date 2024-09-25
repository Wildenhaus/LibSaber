using System.Numerics;
using LibSaber.Common;
using LibSaber.Extensions;
using LibSaber.SpaceMarine2.Enumerations;
using LibSaber.SpaceMarine2.Structures;
using LibSaber.SpaceMarine2.Structures.Geometry;
using LibSaber.IO;

namespace LibSaber.SpaceMarine2.Serialization.Geometry
{

  public sealed class VertexSerializer : GeometryElementSerializer<Vertex>
  {

    #region Data Members

    private bool _hasNormal;
    private bool _hasSkinningData;

    #endregion

    #region Constructor

    public VertexSerializer(GeometryBuffer buffer)
      : base(buffer)
    {
      ASSERT(buffer.Flags.HasFlag(GeometryBufferFlags._VERT),
        "Buffer does not specify _VERT in its flags.");

      _hasNormal = DetermineIfHasNormal(buffer);
      _hasSkinningData = DetermineIfHasSkinningData(buffer);
    }

    #endregion

    #region Overrides

    public override Vertex Deserialize(NativeReader reader)
    {
      var vertex = new Vertex();

      ReadPosition(reader, ref vertex);

      if (_hasNormal)
        ReadNormal(reader, ref vertex);
      if (_hasSkinningData)
        ReadSkinningData(reader, ref vertex);
      if (_hasNormal)
        ReadUncompressedNormal(reader, ref vertex);

      return vertex;
    }

    public override IEnumerable<Vertex> DeserializeRange(NativeReader reader, int startIndex, int endIndex)
    {
      var startOffset = Buffer.StartOffset + (startIndex * Buffer.ElementSize);
      var length = endIndex - startIndex;

      reader.Position = startOffset;

      for (var i = 0; i < length; i++)
      {
        var startPos = reader.Position;
        yield return Deserialize(reader);
        var endPos = reader.Position;
        var lenRead = endPos - startPos;
        reader.Position = startPos + Buffer.ElementSize;
        //ASSERT(lenRead == Buffer.ElementSize);
      }
    }

    #endregion

    #region Private Methods

    private bool DetermineIfHasSkinningData(GeometryBuffer buffer)
    {
      return Flags.HasFlag(GeometryBufferFlags._WEIGHT1)
          || Flags.HasFlag(GeometryBufferFlags._WEIGHT2)
          || Flags.HasFlag(GeometryBufferFlags._WEIGHT3)
          || Flags.HasFlag(GeometryBufferFlags._WEIGHT4WithBoneIds)
          || Flags.HasFlag(GeometryBufferFlags._WEIGHT8WithBones)
          || Flags.HasFlag(GeometryBufferFlags._INDEX);
    }

    private bool DetermineIfHasNormal(GeometryBuffer buffer)
    {
      return Flags.HasFlag(GeometryBufferFlags._WEIGHT8WithBones)
          || Flags.HasFlag(GeometryBufferFlags._NORM_IN_VERT4)
          || Flags.HasFlag(GeometryBufferFlags._COMPRESSED_NORM);
    }

    private void ReadPosition(NativeReader reader, ref Vertex vertex)
    {
      if (Flags.HasFlag(GeometryBufferFlags._COMPRESSED_VERT))
      {
        vertex.Position = new Vector4(
          x: reader.ReadInt16().SNormToFloat(),
          y: reader.ReadInt16().SNormToFloat(),
          z: reader.ReadInt16().SNormToFloat(),
          w: 1);
      }
      else
      {
        vertex.Position = new Vector4(
          x: reader.ReadFloat32(),
          y: reader.ReadFloat32(),
          z: reader.ReadFloat32(),
          w: 1);
      }
    }

    private void ReadNormal(NativeReader reader, ref Vertex vertex)
    {
      if (Flags.HasFlag(GeometryBufferFlags._NORM_IN_VERT4))
      {
        if (Flags.HasFlag(GeometryBufferFlags._COMPRESSED_VERT))
          vertex.Normal = DecompressNormalFromInt16(reader.ReadInt16());
        else
          vertex.Normal = DecompressNormalFromFloat(reader.ReadFloat32());
      }
      else
      {
        _ = reader.ReadInt16();
      }
    }

    private void ReadUncompressedNormal(NativeReader reader, ref Vertex vertex)
    {
      if (Flags.HasFlag(GeometryBufferFlags._NORM_IN_VERT4))
        return;

      if (!Flags.HasFlag(GeometryBufferFlags._COMPRESSED_NORM))
        return;

      var x = reader.ReadFloat32();
      var y = reader.ReadFloat32();
      var z = reader.ReadFloat32();
      vertex.Normal = new Vector4(x, y, z, 1);
    }

    private void ReadSkinningData(NativeReader reader, ref Vertex vertex)
    {
      // TODO: Idk if these data types are right, and/or what to do with them.
      if (Flags.HasFlag(GeometryBufferFlags._WEIGHT1))
        vertex.Weight1 = reader.ReadByte().UNormToFloat();
      if (Flags.HasFlag(GeometryBufferFlags._WEIGHT2))
        vertex.Weight2 = reader.ReadByte().UNormToFloat();
      if (Flags.HasFlag(GeometryBufferFlags._WEIGHT3))
        vertex.Weight3 = reader.ReadByte().UNormToFloat();
      if (Flags.HasFlag(GeometryBufferFlags._WEIGHT4WithBoneIds))
      {
        var isShort = !Flags.HasFlag(GeometryBufferFlags._WEIGHT8WithBones);

        vertex.Weight1 = reader.ReadByte().UNormToFloat();
        vertex.Weight2 = reader.ReadByte().UNormToFloat();
        vertex.Weight3 = reader.ReadByte().UNormToFloat();
        vertex.Weight4 = reader.ReadByte().UNormToFloat();
        vertex.Index1 = isShort ? reader.ReadInt16() : reader.ReadByte();
        vertex.Index2 = isShort ? reader.ReadInt16() : reader.ReadByte();
        vertex.Index3 = isShort ? reader.ReadInt16() : reader.ReadByte();
        vertex.Index4 = isShort ? reader.ReadInt16() : reader.ReadByte();
      }
      else if (Flags.HasFlag(GeometryBufferFlags._WEIGHT8WithBones))
      {
        vertex.Weight1 = reader.ReadByte().UNormToFloat();
        vertex.Weight2 = reader.ReadByte().UNormToFloat();
        vertex.Weight3 = reader.ReadByte().UNormToFloat();
        vertex.Weight4 = reader.ReadByte().UNormToFloat();
        _ = reader.ReadInt32(); //skip
        vertex.Index1 = reader.ReadByte();
        vertex.Index2 = reader.ReadByte();
        vertex.Index3 = reader.ReadByte();
        vertex.Index4 = reader.ReadByte();
        _ = reader.ReadInt32(); // skip
      }

      if (Flags.HasFlag(GeometryBufferFlags._INDEX))
      {
        vertex.Index1 = reader.ReadByte();
        vertex.Index2 = reader.ReadByte();
        vertex.Index3 = reader.ReadByte();
        vertex.Index4 = reader.ReadByte();
      }
    }

    private Vector4 DecompressNormalFromInt16(short w)
    {
      /* In common_input.vsh, if the vertex IS compressed, they're unpacking the normal like so:
       *  xz  = (-1.f + 2.f * frac( float2(1.f/181, 1.f/181.0/181.0) * abs(w))) * float2(181.f/179.f, 181.f/180.f);
       *  y   = sign(inInt16Value) * sqrt(saturate(1.f - tmp.x*tmp.x - tmp.z*tmp.z));
       */

      var negativeIdentity = new Vector2(-1.0f);
      if (w == -32768)
        w = 0;

      var xz = (negativeIdentity + 2.0f * SaberMath.Frac(new Vector2(1.0f / 181, 1.0f / 181.0f / 181.0f) * Math.Abs(w)));
      xz *= new Vector2(181.0f / 179.0f, 181.0f / 180.0f);

      var yTmp = SaberMath.Sign(w) * Math.Sqrt(SaberMath.Saturate(1.0f - xz.X * xz.X - xz.Y * xz.Y));

      var x = (float)xz.X;
      var y = (float)yTmp;
      var z = (float)xz.Y;

      ASSERT(x < 1.1f && x > -1.1f);
      ASSERT(y < 1.1f && y > -1.1f);
      ASSERT(z < 1.1f && z > -1.1f);
      ASSERT(!float.IsNaN(x));
      ASSERT(!float.IsNaN(y));
      ASSERT(!float.IsNaN(z));

      return new Vector4(x, y, z, SaberMath.Sign(w)); // TODO: Should this W be 1?
    }

    private Vector4 DecompressNormalFromFloat(float w)
    {
      /* In common_input.vsh, if the vertex isn't compressed, they're unpacking the normal like so:
       *  norm = -1.0f + 2.f * float3(1/256.0, 1/256.0/256.0, 1/256.0/256.0/256.0) * w);
       */
      var negativeIdentity = new Vector3(-1.0f);
      var divisor = new Vector3(0.00390625f, 0.0000152587890625f, 0.000000059604644775390625f);
      var result = negativeIdentity + 2.0f * SaberMath.Frac(divisor * w);

      return new Vector4(result, 1); // TODO: Verify all this math
    }

    #endregion

  }

}
