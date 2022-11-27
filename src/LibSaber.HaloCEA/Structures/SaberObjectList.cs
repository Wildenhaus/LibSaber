using LibSaber.IO;
using LibSaber.Serialization;

namespace LibSaber.HaloCEA.Structures
{

  public struct SaberObjectList : ISerialData<SaberObjectList>
  {

    #region Data Members

    public int Count;
    public List<SaberObject> Objects;

    #endregion

    #region Serialization

    public static SaberObjectList Deserialize( NativeReader reader, ISerializationContext context )
    {
      var objectList = new SaberObjectList();

      var sentinelReader = new SentinelReader( reader );
      sentinelReader.Next();
      if ( sentinelReader.SentinelId == SentinelIds.Sentinel_012C )
        ReadObjectList( ref objectList, reader, context );
      else
      {
        reader.Position -= 6;
        var objects = objectList.Objects = new List<SaberObject>( 1 );
        objects.Add( SaberObject.Deserialize( reader, context ) );
        return objectList;
      }

      return objectList;
    }

    private static void ReadObjectList( ref SaberObjectList objectList, NativeReader reader, ISerializationContext context )
    {
      var count = objectList.Count = reader.ReadInt32();

      var sentinelReader = new SentinelReader( reader );
      var objects = objectList.Objects = new List<SaberObject>( count );
      while ( sentinelReader.Next() )
      {
        switch ( sentinelReader.SentinelId )
        {
          case 0x00F0:
            var obj = SaberObject.Deserialize( reader, context );
            objectList.Objects.Add( obj );
            ASSERT( objectList.Objects.Count <= objectList.Count, "Object list exceeded expected count." );
            break;

          case SentinelIds.Delimiter:
            ASSERT( objectList.Objects.Count == objectList.Count, "Unexpected end of object list." );
            return;

          default:
            sentinelReader.ReportUnknownSentinel();
            break;
        }
      }
    }

    #endregion

  }

}
