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

    private Vector3 _position;
    private Vector3 _scale;

    #endregion

    #region Constructor

    public VertexSerializer(GeometryBuffer buffer, Vector3? position = null, Vector3? scale = null)
      : base(buffer)
    {
      ASSERT((Flags & FVFFlags.VERT) != 0,
        "Buffer does not specify VERT in its flags.");

      _position = position ?? new Vector3(0, 0, 0);
      _scale = scale ?? new Vector3(1, 1, 1);
    }

    #endregion

    #region Overrides

    public override Vertex Deserialize(NativeReader reader)
    {
      var vertex = new Vertex();

      ReadPosition(reader, ref vertex);
      ReadSkinningData(reader, ref vertex);
      ReadMaskingFlags(reader, ref vertex);
      ReadNormal(reader, ref vertex);

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
    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
    private void ReadPosition(NativeReader reader, ref Vertex vertex)
    {
      if ((Flags & FVFFlags.VERT) != 0)
      {
        var posX = _position.X;
        var posY = _position.Y;
        var posZ = _position.Z;
        var scaleX = _scale.X;
        var scaleY = _scale.Y;
        var scaleZ = _scale.Z;

        ASSERT((Flags & FVFFlags.VERT_2D) == 0, "VERT_2D is present.");

        if ((Flags & FVFFlags.VERT_COMPR) != 0)
        {
          vertex.Position = new Vector4(
            x: reader.ReadInt16().SNormToFloat() * scaleX + posX,
            y: reader.ReadInt16().SNormToFloat() * scaleY + posY,
            z: reader.ReadInt16().SNormToFloat() * scaleZ + posZ,
            w: 1);

          if ((Flags & FVFFlags.NORM_IN_VERT4) != 0)
          {
            vertex.HasNormal = true;
            vertex.Normal = DecompressNormalFromInt16(reader.ReadInt16());
          }
          else
          {
            _ = reader.ReadInt16();
          }
        }
        else
        {
          vertex.Position = new Vector4(
          x: reader.ReadFloat32() * scaleX + posX,
          y: reader.ReadFloat32() * scaleY + posY,
            z: reader.ReadFloat32() * scaleZ + posZ,
            w: 1);

          if ((Flags & FVFFlags.NORM_IN_VERT4) != 0)
          {
            vertex.HasNormal = true;
            vertex.Normal = DecompressNormalFromFloat(reader.ReadFloat32());
          }
        }
      }
    }

    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
    private void ReadSkinningData(NativeReader reader, ref Vertex vertex)
    {
      const FVFFlags HAS_WEIGHTS = FVFFlags.WEIGHT4 | FVFFlags.WEIGHT8;
      if ((Flags & HAS_WEIGHTS) != 0)
      {
        vertex.HasWeight1 = true;
        vertex.Weight1 = reader.ReadByte().UNormToFloat();
        vertex.HasWeight2 = true;
        vertex.Weight2 = reader.ReadByte().UNormToFloat();
        vertex.HasWeight3 = true;
        vertex.Weight3 = reader.ReadByte().UNormToFloat();
        vertex.HasWeight4 = true;
        vertex.Weight4 = reader.ReadByte().UNormToFloat();

        if ((Flags & FVFFlags.WEIGHT8) != 0)
        {
          vertex.HasWeight5 = true;
          vertex.Weight5 = reader.ReadByte().UNormToFloat();
          vertex.HasWeight6 = true;
          vertex.Weight6 = reader.ReadByte().UNormToFloat();
          vertex.HasWeight7 = true;
          vertex.Weight7 = reader.ReadByte().UNormToFloat();
          vertex.HasWeight8 = true;
          vertex.Weight8 = reader.ReadByte().UNormToFloat();
        }
      }

      const FVFFlags HAS_INDICES = FVFFlags.INDICES | FVFFlags.INDICES16;
      if ((Flags & HAS_INDICES) != 0)
      {
        if ((Flags & FVFFlags.INDICES16) != 0)
        {
          vertex.Index1 = reader.ReadInt16();
          vertex.Index2 = reader.ReadInt16();
          vertex.Index3 = reader.ReadInt16();
          vertex.Index4 = reader.ReadInt16();
          if ((Flags & FVFFlags.WEIGHT8) != 0)
          {
            vertex.Index5 = reader.ReadInt16();
            vertex.Index6 = reader.ReadInt16();
            vertex.Index7 = reader.ReadInt16();
            vertex.Index8 = reader.ReadInt16();
          }
        }
        else
        {
          vertex.Index1 = reader.ReadByte();
          vertex.Index2 = reader.ReadByte();
          vertex.Index3 = reader.ReadByte();
          vertex.Index4 = reader.ReadByte();
          if ((Flags & FVFFlags.WEIGHT8) != 0)
          {
            vertex.Index5 = reader.ReadByte();
            vertex.Index6 = reader.ReadByte();
            vertex.Index7 = reader.ReadByte();
            vertex.Index8 = reader.ReadByte();
          }
        }
      }
    }

    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
    private void ReadMaskingFlags(NativeReader reader, ref Vertex vertex)
    {
      if ((Flags & FVFFlags.MASKING_FLAGS) != 0)
      {
        _ = reader.ReadInt32();
      }
    }

    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
    private void ReadNormal(NativeReader reader, ref Vertex vertex)
    {
      if ((Flags & FVFFlags.NORM) != 0)
      {
        if ((Flags & FVFFlags.NORM_IN_VERT4) != 0)
          return;

        if ((Flags & FVFFlags.NORM_COMPR) != 0)
        {
          vertex.HasNormal = true;
          vertex.Normal = DecompressNormalFromInt16(reader.ReadInt16()); // TODO
        }
        else
        {
          vertex.HasNormal = true;
          vertex.Normal = new(
          x: reader.ReadFloat32(),
          y: reader.ReadFloat32(),
            z: reader.ReadFloat32(),
            w: 1);
        }
      }
    }

    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
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

    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
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
