﻿using LibSaber.Halo2A.IO;
using LibSaber.Halo2A.Structures;
using LibSaber.IO;
using LibSaber.Serialization;

namespace LibSaber.Halo2A.Serialization
{

  public sealed class CdListSerializer : H2ASerializerBase<List<CdListEntry>>
  {

    #region Overrides

    public override List<CdListEntry> Deserialize( NativeReader reader, ISerializationContext context )
    {
      ReadHeader( reader );
      var lgName = reader.ReadLengthPrefixedString32();

      var count = reader.ReadInt32();
      var unk = reader.ReadInt32();

      var list = new List<CdListEntry>( count );
      for ( var i = 0; i < count; i++ )
        list.Add( new CdListEntry() );

      ReadNames( reader, list );
      ASSERT( reader.ReadByte() == 0 );
      ReadMatrices( reader, list );
      ReadAffixes( reader, list );
      ReadPsSection( reader, list );

      return list;
    }

    #endregion

    #region Private Methods

    private void ReadHeader( NativeReader reader )
    {
      const ulong SIGNATURE_1SERh2a2 = 0x3261326852455331;
      const ulong SIGNATURE_cd_list = 0x7473696C5F64635F;

      ASSERT( reader.ReadUInt64() == SIGNATURE_1SERh2a2, "Invalid signature." );
      ASSERT( reader.ReadUInt64() == SIGNATURE_cd_list, "Invalid signature." );

      reader.Position += 0x44;
    }

    private void ReadNames( NativeReader reader, List<CdListEntry> list )
    {
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var entry in list )
        entry.Name = reader.ReadLengthPrefixedString32();
    }

    private void ReadMatrices( NativeReader reader, List<CdListEntry> list )
    {
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var entry in list )
        entry.Matrix = reader.ReadMatrix4x4();
    }

    private void ReadAffixes( NativeReader reader, List<CdListEntry> list )
    {
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var entry in list )
        entry.Affixes = reader.ReadLengthPrefixedString32();
    }

    private void ReadPsSection( NativeReader reader, List<CdListEntry> list )
    {
      if ( reader.ReadByte() == 0 )
        return;

      var flags = reader.ReadBitArray( list.Count );

      for ( var i = 0; i < list.Count; i++ )
      {
        if ( !flags[ i ] )
          continue;

        var entry = list[ i ];
        var endOfBlock = false;

        var sentinelReader = new SentinelReader( reader );
        while ( sentinelReader.Next() )
        {
          switch ( ( ushort ) sentinelReader.SentinelId )
          {
            case 0x0000:
              entry.PS = reader.ReadLengthPrefixedString32();
              break;

            case 0xFFFF:
              endOfBlock = true;
              break;

            default:
              sentinelReader.ReportUnknownSentinel();
              break;
          }

          if ( endOfBlock )
            break;
        }
      }
    }

    #endregion

  }

}
