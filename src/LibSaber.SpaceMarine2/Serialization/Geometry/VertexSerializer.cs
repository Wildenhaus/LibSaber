using System.Numerics;
using LibSaber.Common;
using LibSaber.Extensions;
using LibSaber.SpaceMarine2.Enumerations;
using LibSaber.SpaceMarine2.Structures;
using LibSaber.SpaceMarine2.Structures.Geometry;
using LibSaber.IO;
using System.Diagnostics;

namespace LibSaber.SpaceMarine2.Serialization.Geometry
{

  public sealed class VertexSerializer : GeometryElementSerializer<Vertex>
  {

    #region Constructor

    public VertexSerializer(GeometryBuffer buffer)
      : base(buffer)
    {
      ASSERT(buffer.Flags.HasFlag(GeometryBufferFlags._VERT),
        "Buffer does not specify _VERT in its flags.");
    }

    #endregion

    #region Overrides

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vertex Deserialize(NativeReader reader, GeometryBufferFlags flags)
    {
      var vertex = new Vertex();

      #region Position

      if (flags.HasFlag(GeometryBufferFlags._VERT))
      {
        if (flags.HasFlag(GeometryBufferFlags._VERT_COMPR))
        {
          vertex.Position.X = reader.ReadInt16().SNormToFloat();
          vertex.Position.Y = reader.ReadInt16().SNormToFloat();
          vertex.Position.Z = reader.ReadInt16().SNormToFloat();

          if (flags.HasFlag(GeometryBufferFlags._NORM_IN_VERT4))
            vertex.Normal = DecompressNormalFromInt16(reader.ReadInt16());
        }
        else
        {
          vertex.Position.X = reader.ReadFloat32();
          vertex.Position.Y = reader.ReadFloat32();
          vertex.Position.Z = reader.ReadFloat32();
          vertex.Position.W = 1;
        }
      }

      #endregion

      #region Skinning Data

      if (flags.HasFlag(GeometryBufferFlags._WEIGHT4))
      {
        vertex.Weight1 = reader.ReadByte().UNormToFloat();
        vertex.Weight2 = reader.ReadByte().UNormToFloat();
        vertex.Weight3 = reader.ReadByte().UNormToFloat();
        vertex.Weight4 = reader.ReadByte().UNormToFloat();
      }
      //else if (Flags.HasFlag(GeometryBufferFlags._WEIGHT8))
      //{
      //  vertex.Weight1 = reader.ReadByte().UNormToFloat();
      //  vertex.Weight2 = reader.ReadByte().UNormToFloat();
      //  vertex.Weight3 = reader.ReadByte().UNormToFloat();
      //  vertex.Weight4 = reader.ReadByte().UNormToFloat();
      //}

      if(flags.HasFlag(GeometryBufferFlags._INDICES4))
      {
        vertex.Index1 = reader.ReadByte();
        vertex.Index2 = reader.ReadByte();
        vertex.Index3 = reader.ReadByte();
        vertex.Index4 = reader.ReadByte();
      }

      //if (Flags.HasFlag(GeometryBufferFlags._INDICES))
      //{
      //  var indexIs16Bit = Flags.HasFlag(GeometryBufferFlags._WEIGHT8);

      //  if (Flags.HasFlag(GeometryBufferFlags._WEIGHT4))
      //  {
      //    vertex.Index1 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      //    vertex.Index2 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      //    vertex.Index3 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      //    vertex.Index4 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      //  }
      //  else if (Flags.HasFlag(GeometryBufferFlags._WEIGHT8))
      //  {
      //    vertex.Index1 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      //    vertex.Index2 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      //    vertex.Index3 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      //    vertex.Index4 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      //  }
      //}

      #endregion

      #region Normal

      if (flags.HasFlag(GeometryBufferFlags._NORM)
        && !flags.HasFlag(GeometryBufferFlags._NORM_IN_VERT4))
      {
        vertex.Normal = new Vector4(
          x: reader.ReadFloat32(),
          y: reader.ReadFloat32(),
          z: reader.ReadFloat32(),
          w: 1
        );
      }

      #endregion

      return vertex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Vertex Deserialize(SpanReader reader, GeometryBufferFlags flags)
    {
      var vertex = new Vertex();

      #region Position

      if (flags.HasFlag(GeometryBufferFlags._VERT))
      {
        if (flags.HasFlag(GeometryBufferFlags._VERT_COMPR))
        {
          vertex.Position.X = reader.ReadInt16().SNormToFloat();
          vertex.Position.Y = reader.ReadInt16().SNormToFloat();
          vertex.Position.Z = reader.ReadInt16().SNormToFloat();

          if (flags.HasFlag(GeometryBufferFlags._NORM_IN_VERT4))
            vertex.Normal = DecompressNormalFromInt16(reader.ReadInt16());
        }
        else
        {
          vertex.Position.X = reader.ReadFloat32();
          vertex.Position.Y = reader.ReadFloat32();
          vertex.Position.Z = reader.ReadFloat32();
          vertex.Position.W = 1;
        }
      }

      #endregion

      #region Skinning Data

      if (flags.HasFlag(GeometryBufferFlags._WEIGHT4))
      {
        vertex.Weight1 = reader.ReadByte().UNormToFloat();
        vertex.Weight2 = reader.ReadByte().UNormToFloat();
        vertex.Weight3 = reader.ReadByte().UNormToFloat();
        vertex.Weight4 = reader.ReadByte().UNormToFloat();
      }
      //else if (Flags.HasFlag(GeometryBufferFlags._WEIGHT8))
      //{
      //  vertex.Weight1 = reader.ReadByte().UNormToFloat();
      //  vertex.Weight2 = reader.ReadByte().UNormToFloat();
      //  vertex.Weight3 = reader.ReadByte().UNormToFloat();
      //  vertex.Weight4 = reader.ReadByte().UNormToFloat();
      //}

      if (flags.HasFlag(GeometryBufferFlags._INDICES4))
      {
        vertex.Index1 = reader.ReadByte();
        vertex.Index2 = reader.ReadByte();
        vertex.Index3 = reader.ReadByte();
        vertex.Index4 = reader.ReadByte();
      }

      //if (Flags.HasFlag(GeometryBufferFlags._INDICES))
      //{
      //  var indexIs16Bit = Flags.HasFlag(GeometryBufferFlags._WEIGHT8);

      //  if (Flags.HasFlag(GeometryBufferFlags._WEIGHT4))
      //  {
      //    vertex.Index1 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      //    vertex.Index2 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      //    vertex.Index3 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      //    vertex.Index4 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      //  }
      //  else if (Flags.HasFlag(GeometryBufferFlags._WEIGHT8))
      //  {
      //    vertex.Index1 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      //    vertex.Index2 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      //    vertex.Index3 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      //    vertex.Index4 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      //  }
      //}

      #endregion

      #region Normal

      if (flags.HasFlag(GeometryBufferFlags._NORM)
        && !flags.HasFlag(GeometryBufferFlags._NORM_IN_VERT4))
      {
        vertex.Normal = new Vector4(
          x: reader.ReadFloat32(),
          y: reader.ReadFloat32(),
          z: reader.ReadFloat32(),
          w: 1
        );
      }

      #endregion

      return vertex;
    }

    public override Vertex Deserialize(NativeReader reader)
    {
      var vertex = new Vertex();

      //#region Position

      //if (flags.HasFlag(GeometryBufferFlags._VERT))
      //{
      //  if (flags.HasFlag(GeometryBufferFlags._VERT_COMPR))
      //  {
      //    vertex.Position.X = reader.ReadInt16().SNormToFloat();
      //    vertex.Position.Y = reader.ReadInt16().SNormToFloat();
      //    vertex.Position.Z = reader.ReadInt16().SNormToFloat();

      //    if (flags.HasFlag(GeometryBufferFlags._NORM_IN_VERT4))
      //      vertex.Normal = DecompressNormalFromInt16(reader.ReadInt16());
      //  }
      //  else
      //  {
      //    vertex.Position.X = reader.ReadFloat32();
      //    vertex.Position.Y = reader.ReadFloat32();
      //    vertex.Position.Z = reader.ReadFloat32();
      //    vertex.Position.W = 1;
      //  }
      //}

      //#endregion

      //#region Skinning Data

      //if (flags.HasFlag(GeometryBufferFlags._WEIGHT4))
      //{
      //  vertex.Weight1 = reader.ReadByte().UNormToFloat();
      //  vertex.Weight2 = reader.ReadByte().UNormToFloat();
      //  vertex.Weight3 = reader.ReadByte().UNormToFloat();
      //  vertex.Weight4 = reader.ReadByte().UNormToFloat();
      //}
      ////else if (Flags.HasFlag(GeometryBufferFlags._WEIGHT8))
      ////{
      ////  vertex.Weight1 = reader.ReadByte().UNormToFloat();
      ////  vertex.Weight2 = reader.ReadByte().UNormToFloat();
      ////  vertex.Weight3 = reader.ReadByte().UNormToFloat();
      ////  vertex.Weight4 = reader.ReadByte().UNormToFloat();
      ////}

      //if (flags.HasFlag(GeometryBufferFlags._INDICES4))
      //{
      //  vertex.Index1 = reader.ReadByte();
      //  vertex.Index2 = reader.ReadByte();
      //  vertex.Index3 = reader.ReadByte();
      //  vertex.Index4 = reader.ReadByte();
      //}

      ////if (Flags.HasFlag(GeometryBufferFlags._INDICES))
      ////{
      ////  var indexIs16Bit = Flags.HasFlag(GeometryBufferFlags._WEIGHT8);

      ////  if (Flags.HasFlag(GeometryBufferFlags._WEIGHT4))
      ////  {
      ////    vertex.Index1 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      ////    vertex.Index2 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      ////    vertex.Index3 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      ////    vertex.Index4 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      ////  }
      ////  else if (Flags.HasFlag(GeometryBufferFlags._WEIGHT8))
      ////  {
      ////    vertex.Index1 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      ////    vertex.Index2 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      ////    vertex.Index3 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      ////    vertex.Index4 = indexIs16Bit ? reader.ReadInt16() : reader.ReadByte();
      ////  }
      ////}

      //#endregion

      //#region Normal

      //if (flags.HasFlag(GeometryBufferFlags._NORM)
      //  && !flags.HasFlag(GeometryBufferFlags._NORM_IN_VERT4))
      //{
      //  vertex.Normal = new Vector4(
      //    x: reader.ReadFloat32(),
      //    y: reader.ReadFloat32(),
      //    z: reader.ReadFloat32(),
      //    w: 1
      //  );
      //}

      //#endregion

      return vertex;
    }

    public override IEnumerable<Vertex> DeserializeRange(NativeReader reader, int startIndex, int endIndex)
    {
      var startOffset = Buffer.StartOffset + (startIndex * Buffer.ElementSize);
      var length = endIndex - startIndex;

      if (Flags.HasFlag(GeometryBufferFlags.Unk_07))
        Debugger.Break();
      if (Flags.HasFlag(GeometryBufferFlags.Unk_09))
        Debugger.Break();

      reader.Position = startOffset;

      var flags = Flags;
      for (var i = 0; i < length; i++)
      {
        var startPos = reader.Position;
        var spanReader = reader.GetSpanReader(Buffer.ElementSize);
        var vertex = Deserialize(spanReader, flags);
        yield return vertex;
        var endPos = reader.Position;
        var lenRead = endPos - startPos;
        reader.Position = startPos + Buffer.ElementSize;

        //if (lenRead != Buffer.ElementSize)
        //  ASSERT(lenRead == Buffer.ElementSize);
      }
    }

    #endregion

    #region Private Methods

    //private void ReadNormal(NativeReader reader, ref Vertex vertex)
    //{
    //  if (Flags.HasFlag(GeometryBufferFlags._NORM_IN_VERT4))
    //  {
    //    if (Flags.HasFlag(GeometryBufferFlags._VERT_COMPR))
    //      vertex.Normal = DecompressNormalFromInt16(reader.ReadInt16());
    //    else
    //      vertex.Normal = DecompressNormalFromFloat(reader.ReadFloat32());
    //  }
    //  else
    //  {
    //    _ = reader.ReadInt16();
    //  }
    //}

    //private void ReadUncompressedNormal(NativeReader reader, ref Vertex vertex)
    //{
    //  if (Flags.HasFlag(GeometryBufferFlags._NORM_IN_VERT4))
    //    return;

    //  if (!Flags.HasFlag(GeometryBufferFlags._NORM))
    //    return;

    //  var x = reader.ReadFloat32();
    //  var y = reader.ReadFloat32();
    //  var z = reader.ReadFloat32();
    //  vertex.Normal = new Vector4(x, y, z, 1);
    //}

    //private void ReadSkinningData(NativeReader reader, ref Vertex vertex)
    //{
    //  // TODO: Idk if these data types are right, and/or what to do with them.
    //  //if (Flags.HasFlag(GeometryBufferFlags._WEIGHT1))
    //  //  vertex.Weight1 = reader.ReadByte().UNormToFloat();
    //  //if (Flags.HasFlag(GeometryBufferFlags._WEIGHT2))
    //  //  vertex.Weight2 = reader.ReadByte().UNormToFloat();
    //  //if (Flags.HasFlag(GeometryBufferFlags._WEIGHT3))
    //  //  vertex.Weight3 = reader.ReadByte().UNormToFloat();
    //  if (Flags.HasFlag(GeometryBufferFlags._Unk_07))
    //  {
    //    var isShort = !Flags.HasFlag(GeometryBufferFlags._WEIGHT4);

    //    vertex.Weight1 = reader.ReadByte().UNormToFloat();
    //    vertex.Weight2 = reader.ReadByte().UNormToFloat();
    //    vertex.Weight3 = reader.ReadByte().UNormToFloat();
    //    vertex.Weight4 = reader.ReadByte().UNormToFloat();
    //    vertex.Index1 = isShort ? reader.ReadInt16() : reader.ReadByte();
    //    vertex.Index2 = isShort ? reader.ReadInt16() : reader.ReadByte();
    //    vertex.Index3 = isShort ? reader.ReadInt16() : reader.ReadByte();
    //    vertex.Index4 = isShort ? reader.ReadInt16() : reader.ReadByte();
    //  }
    //  else if (Flags.HasFlag(GeometryBufferFlags._WEIGHT4))
    //  {
    //    vertex.Weight1 = reader.ReadByte().UNormToFloat();
    //    vertex.Weight2 = reader.ReadByte().UNormToFloat();
    //    vertex.Weight3 = reader.ReadByte().UNormToFloat();
    //    vertex.Weight4 = reader.ReadByte().UNormToFloat();
    //    _ = reader.ReadInt32(); //skip
    //    vertex.Index1 = reader.ReadByte();
    //    vertex.Index2 = reader.ReadByte();
    //    vertex.Index3 = reader.ReadByte();
    //    vertex.Index4 = reader.ReadByte();
    //    _ = reader.ReadInt32(); // skip
    //  }

    //  if (Flags.HasFlag(GeometryBufferFlags._WEIGHT8))
    //  {
    //    vertex.Index1 = reader.ReadByte();
    //    vertex.Index2 = reader.ReadByte();
    //    vertex.Index3 = reader.ReadByte();
    //    vertex.Index4 = reader.ReadByte();
    //  }
    //}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector4 DecompressNormalFromInt16(short w)
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

      //ASSERT(x < 1.1f && x > -1.1f);
      //ASSERT(y < 1.1f && y > -1.1f);
      //ASSERT(z < 1.1f && z > -1.1f);
      //ASSERT(!float.IsNaN(x));
      //ASSERT(!float.IsNaN(y));
      //ASSERT(!float.IsNaN(z));

      return new Vector4(x, y, z, SaberMath.Sign(w)); // TODO: Should this W be 1?
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector4 DecompressNormalFromFloat(float w)
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
