using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;
using LibSaber.Shared.Structures;

namespace LibSaber.HaloCEA.Structures
{

  [Sentinel( SentinelIds.Sentinel_02FD )]
  [SaberInternalName( "animSEQ" )]
  public struct CEAAnimationSequence : ISerialData<CEAAnimationSequence>
  {

    #region Data Members

    [Sentinel( SentinelIds.Sentinel_02FE )]
    [SaberInternalName( "name" )]
    public string Name;

    [Sentinel( SentinelIds.Sentinel_02F3 )]
    public float UnkTimeSec;

    [Sentinel( SentinelIds.Sentinel_02F4 )]
    public float UnkLenFrame;

    [Sentinel( SentinelIds.Sentinel_02FF )]
    public float UnkStartFrame;

    [Sentinel( SentinelIds.Sentinel_0300 )]
    public float UnkEndFrame;

    [Sentinel( SentinelIds.Sentinel_0301 )]
    public float UnkOffsetFrame;

    [Sentinel( SentinelIds.Sentinel_0307 )]
    [SaberInternalName( "bbox" )]
    public Box BoundingBox;

    [Sentinel( SentinelIds.Sentinel_0314 )]
    public List<ActionFrame> ActionFrames; // actionFrames

    #endregion

    #region Serialization

    public static CEAAnimationSequence Deserialize( NativeReader reader, ISerializationContext context )
    {
      var animSeq = new CEAAnimationSequence();

      var sentinelReader = new SentinelReader( reader );
      while ( sentinelReader.Next() )
      {
        switch ( sentinelReader.SentinelId )
        {
          case 0x02FE:
            animSeq.Name = reader.ReadNullTerminatedString();
            break;
          case 0x02F3:
            animSeq.UnkTimeSec = reader.ReadFloat32();
            break;
          case 0x02F4:
            animSeq.UnkLenFrame = reader.ReadFloat32();
            break;
          case 0x02FF:
            animSeq.UnkStartFrame = reader.ReadFloat32();
            break;
          case 0x0300:
            animSeq.UnkEndFrame = reader.ReadFloat32();
            break;
          case 0x0301:
            animSeq.UnkOffsetFrame = reader.ReadFloat32();
            break;
          case 0x0307:
            _ = reader.ReadInt32(); // unk
            animSeq.BoundingBox = Box.Deserialize( reader, context );
            break;
          case 0x0314:
            animSeq.ActionFrames = DataList<ActionFrame>.Deserialize( reader, context );
            break;

          case 0x0001:
            return animSeq;

          default:
            sentinelReader.ReportUnknownSentinel();
            break;
        }
      }

      return animSeq;
    }

    #endregion

  }

}
