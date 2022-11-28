using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;

namespace LibSaber.HaloCEA.Structures
{

  public class SaberScene : ISerialData<SaberScene>
  {

    #region Data Members

    [Sentinel( SentinelIds.Sentinel_03C0 )]
    public Guid? Data_03C0;

    [Sentinel( SentinelIds.TextureList )]
    public TextureList TextureList;

    [Sentinel( SentinelIds.Sentinel_021F )]
    public Data_021F Data_021F;

    [Sentinel( SentinelIds.Sentinel_0482)]
    [SaberInternalName( "psSECTION" )]
    public Data_0482 psSECTION;

    [Sentinel( SentinelIds.Sentinel_0484)]
    public Data_0484 Data_0484;

    [Sentinel( SaberSceneSentinelIds.ObjectList )]
    public SaberObjectList ObjectList;

    [Sentinel( SentinelIds.Sentinel_01EA )]
    public Data_01EA TemplateList;

    [Sentinel( SentinelIds.Sentinel_01B8 )]
    public Data_01B8 Data_01B8;

    [Sentinel( SentinelIds.Sentinel_0280 )]
    public Data_0280 Data_0280;

    [Sentinel( SentinelIds.Sentinel_0425 )]
    public byte[] Data_0425;

    [Sentinel( SentinelIds.Sentinel_021D )]
    public int Data_021D;

    [Sentinel( SentinelIds.Sentinel_021E )]
    public Data_021E Data_021E;

    #endregion

    #region Serialization

    public static SaberScene Deserialize( NativeReader reader, ISerializationContext context )
    {
      var scene = new SaberScene();

      var sentinelReader = new SentinelReader( reader );
      while ( sentinelReader.Next() )
      {
        switch ( sentinelReader.SentinelId )
        {
          case SentinelIds.Sentinel_03C0:
            scene.Data_03C0 = reader.ReadGuid();
            break;
          case SentinelIds.TextureList:
            scene.TextureList = TextureList.Deserialize( reader, context );
            sentinelReader.BurnSentinel();
            break;
          case SentinelIds.Sentinel_01B8:
            scene.Data_01B8 = Data_01B8.Deserialize( reader, context );
            break;
          case SentinelIds.Sentinel_01EA:
            scene.TemplateList = Data_01EA.Deserialize( reader, context );
            break;
          case SentinelIds.Sentinel_021F:
            scene.Data_021F = Data_021F.Deserialize( reader, context );
            sentinelReader.SetDelimiterFlag();
            break;
          case SentinelIds.Sentinel_0280:
            scene.Data_0280 = Data_0280.Deserialize( reader, context );
            break;
          case SentinelIds.Sentinel_0482:
            scene.psSECTION = Data_0482.Deserialize( reader, context );
            break;
          case SentinelIds.Sentinel_0484:
            scene.Data_0484 = Data_0484.Deserialize( reader, context );
            break;
          case SentinelIds.Sentinel_0425:
            scene.Data_0425 = new byte[ reader.ReadInt32() ];
            reader.Read( scene.Data_0425 );
            break;
          case SentinelIds.Sentinel_021D:
            scene.Data_021D = reader.ReadInt32();
            break;
          case SentinelIds.Sentinel_021E:
            scene.Data_021E = Data_021E.Deserialize( reader, context );
            break;

          case SaberSceneSentinelIds.ObjectList:
            scene.ObjectList = SaberObjectList.Deserialize( reader, context );
            sentinelReader.BurnSentinel();
            break;

          case SentinelIds.Delimiter:
            break;

          default:
            sentinelReader.ReportUnknownSentinel();
            break;
        }
      }

      return scene;
    }

    #endregion

    #region Local Sentinels

    public static class SaberSceneSentinelIds
    {

      public const SentinelId ObjectList = 0x00F0;

    }

    #endregion

  }

}
