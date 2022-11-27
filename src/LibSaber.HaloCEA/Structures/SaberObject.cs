using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;
using LibSaber.Shared.Structures;

namespace LibSaber.HaloCEA.Structures
{

  public class SaberObject : ISerialData<SaberObject>
  {

    #region Data Members

    // Global Sentinels =================================================
    [Sentinel( SentinelIds.Sentinel_03B9 )]
    public Data_03B9 ObjectInfo;

    [Sentinel( SentinelIds.Sentinel_0107 )]
    public Data_0107? UnkSubmeshData;

    [Sentinel( SentinelIds.Sentinel_011D )]
    public Box? BoundingBox;

    [Sentinel( SentinelIds.Sentinel_0115 )]
    public string? Affixes;

    [Sentinel( SentinelIds.Sentinel_0116 )]
    public Data_0116? SkinningData;

    [Sentinel( SentinelIds.Sentinel_012B )]
    public int? ParentId;

    [Sentinel( SentinelIds.Sentinel_012E )]
    public BitSet<short> GeometryFlags;

    [Sentinel( SentinelIds.Sentinel_012F )]
    public Data_012F? UnkMaterialData;

    [Sentinel( SentinelIds.Sentinel_0130 )]
    public Data_0130? InterleavedDataBuffer;

    // Object Sentinels =================================================
    [Sentinel( CEAObjectSentinels.VertexBuffer )]
    public List<CEAVertex> Vertices;

    [Sentinel( CEAObjectSentinels.FaceBuffer )]
    public List<Face> Faces;

    [Sentinel( CEAObjectSentinels.Sentinel_00F8 )]
    public Vector4<byte>? MaterialColor;

    [Sentinel( CEAObjectSentinels.Matrix )]
    public Matrix4<float>? Matrix;

    [Sentinel( CEAObjectSentinels.Sentinel_00FA )]
    public int? UnkBoneId; // Bone ID?

    [Sentinel( CEAObjectSentinels.Sentinel_00FD )]
    public string? UnkScripting; // Scripting?

    #endregion

    #region Serialization

    public static SaberObject Deserialize( NativeReader reader, ISerializationContext context )
    {
      var obj = new SaberObject();
      context.AddObject( obj );

      var sentinelReader = new SentinelReader( reader );
      while ( sentinelReader.Next() )
      {
        switch ( sentinelReader.SentinelId )
        {
          // Global Sentinels =================================================
          case SentinelIds.Sentinel_03B9:
          {
            obj.ObjectInfo = Data_03B9.Deserialize( reader, context );
            break;
          }
          case SentinelIds.Sentinel_0107:
          {
            obj.UnkSubmeshData = Data_0107.Deserialize( reader, context );
            break;
          }
          case SentinelIds.Sentinel_011D:
          {
            _ = reader.ReadInt32();
            obj.BoundingBox = Box.Deserialize( reader, context );
            break;
          }
          case SentinelIds.Sentinel_0115:
          {
            obj.Affixes = reader.ReadNullTerminatedString();
            break;
          }
          case SentinelIds.Sentinel_0116:
          {
            obj.SkinningData = Data_0116.Deserialize( reader, context );
            break;
          }
          case SentinelIds.Sentinel_012B:
          {
            obj.ParentId = reader.ReadInt32();
            break;
          }
          case SentinelIds.Sentinel_012E:
          {
            obj.GeometryFlags = BitSet<short>.Deserialize( reader, context );
            break;
          }
          case SentinelIds.Sentinel_012F:
          {
            obj.UnkMaterialData = Data_012F.Deserialize( reader, context );
            break;
          }
          case SentinelIds.Sentinel_0130:
          {
            obj.InterleavedDataBuffer = Data_0130.Deserialize( reader, context );
            break;
          }

          // Object Sentinels =================================================
          case CEAObjectSentinels.VertexBuffer:
          {
            obj.Vertices = CEAVertexBuffer.Deserialize( reader, context );
            break;
          }
          case CEAObjectSentinels.FaceBuffer:
          {
            obj.Faces = FaceBuffer.Deserialize( reader, context );
            break;
          }
          case CEAObjectSentinels.Sentinel_00F8:
          {
            obj.MaterialColor = Vector4<byte>.Deserialize( reader, context );
            break;
          }
          case CEAObjectSentinels.Matrix:
          {
            obj.Matrix = Matrix4<float>.Deserialize( reader, context );
            break;
          }
          case CEAObjectSentinels.Sentinel_00FA:
          {
            obj.UnkBoneId = reader.ReadInt32();
            break;
          }
          case CEAObjectSentinels.Sentinel_00FD:
          {
            var innerSentinelReader = new SentinelReader( reader );
            innerSentinelReader.Next();

            ASSERT( innerSentinelReader.SentinelId == SentinelIds.Sentinel_01BA,
              "Unexpected Sentinel inside Object Sentinel 00FD: {0:X4}",
              innerSentinelReader.SentinelId );

            obj.UnkScripting = reader.ReadNullTerminatedString();
            break;
          }

          // Default Sentinels ================================================
          case SentinelIds.Delimiter:
            return obj;

          default:
            sentinelReader.ReportUnknownSentinel();
            break;
        }
      }

      return obj;
    }

    #endregion

    #region CEAObject Sentinels

    private static class CEAObjectSentinels
    {
      public const SentinelId Sentinel_00F0 = 0x00F0;
      public const SentinelId VertexBuffer = 0x00F1;
      public const SentinelId FaceBuffer = 0x00F2;
      public const SentinelId Sentinel_00F3 = 0x00F3;
      public const SentinelId Sentinel_00F4 = 0x00F4;
      public const SentinelId Sentinel_00F5 = 0x00F5;
      public const SentinelId Sentinel_00F6 = 0x00F6;
      public const SentinelId Sentinel_00F7 = 0x00F7;
      public const SentinelId Sentinel_00F8 = 0x00F8;
      public const SentinelId Matrix = 0x00F9;
      public const SentinelId Sentinel_00FA = 0x00FA;
      public const SentinelId Sentinel_00FB = 0x00FB;
      public const SentinelId Sentinel_00FC = 0x00FC;
      public const SentinelId Sentinel_00FD = 0x00FD;
    }

    #endregion

  }
}
