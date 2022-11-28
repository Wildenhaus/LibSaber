using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;
using LibSaber.Shared.Structures;

namespace LibSaber.HaloCEA.Structures
{

  [Sentinel( SentinelIds.AnimationSequence )]
  [SaberInternalName( "animSEQ" )]
  public struct AnimationSequence : ISerialData<AnimationSequence>
  {

    #region Data Members

    [Sentinel( SentinelIds.AnimationSequence_Name )]
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

    [Sentinel( SentinelIds.AnimationSequence_ActionFrames )]
    public List<ActionFrame> ActionFrames; // actionFrames

    #endregion

    #region Serialization

    public static AnimationSequence Deserialize( NativeReader reader, ISerializationContext context )
    {
      var animSeq = new AnimationSequence();

      var sentinelReader = new SentinelReader( reader );
      while ( sentinelReader.Next() )
      {
        switch ( sentinelReader.SentinelId )
        {
          case SentinelIds.AnimationSequence_Name:
            animSeq.Name = reader.ReadNullTerminatedString();
            break;
          case SentinelIds.Sentinel_02F3:
            animSeq.UnkTimeSec = reader.ReadFloat32();
            break;
          case SentinelIds.Sentinel_02F4:
            animSeq.UnkLenFrame = reader.ReadFloat32();
            break;
          case SentinelIds.Sentinel_02FF:
            animSeq.UnkStartFrame = reader.ReadFloat32();
            break;
          case SentinelIds.Sentinel_0300:
            animSeq.UnkEndFrame = reader.ReadFloat32();
            break;
          case SentinelIds.Sentinel_0301:
            animSeq.UnkOffsetFrame = reader.ReadFloat32();
            break;
          case SentinelIds.Sentinel_0307:
            _ = reader.ReadInt32(); // unk
            animSeq.BoundingBox = Box.Deserialize( reader, context );
            break;
          case SentinelIds.AnimationSequence_ActionFrames:
            animSeq.ActionFrames = DataList<ActionFrame>.Deserialize( reader, context );
            break;

          case SentinelIds.Delimiter:
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
