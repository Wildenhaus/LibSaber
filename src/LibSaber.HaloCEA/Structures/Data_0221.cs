using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Attributes;

namespace LibSaber.HaloCEA.Structures
{

  [Sentinel( SentinelIds.Sentinel_0221 )]
  public class Data_0221 : ISerialData<Data_0221>
  {

    #region Data Members

    public int Unk_00;
    public int Unk_01;
    public byte[] Unk_02;

    #endregion

    #region Serialization

    public static Data_0221 Deserialize( NativeReader reader, ISerializationContext context )
    {
      /* Not sure what this is, but it looks like a bunch of zero data.
       * 
       * The number of bytes is calculated based on the scalar product of 
       * the first vector in the previous sentinel (0x0220):
       *    Data_0220.Unk_00.X * Data_0220.Unk_00.Y * Unk_0220.Unk_00.Z + 1;
       * 
       * This data is an array of 4-byte elements. Probably either int or float,
       * but since my sample data is all zeros, I can't tell.
       */

      var data = new Data_0221();

      data.Unk_00 = reader.ReadInt32();
      data.Unk_01 = reader.ReadInt32();

      var unkParent = context.GetMostRecentObject<Data_0220>();
      var elemCount = unkParent.Unk_00.X * unkParent.Unk_00.Y * unkParent.Unk_00.Z;
      var unk_02_size = ( elemCount + 1 ) * 4;
      data.Unk_02 = new byte[ unk_02_size ];
      reader.Read( data.Unk_02 );

      /* There's another call that reads 2-byte elements into another array.
       * 
       * The allocation for that data is in Inversion.exe+0xD4A520.
       * It's doing a bunch of weird bit shifting. Might be related to flags?
       * The deserialization function for this sentinel group is inside of a chain
       * of virtual calls that I don't care to try and follow right now.
       * 
       * Skipping for now, but I'll revisit this if it's an issue.
       */

      context.AddObject( data );
      return data;
    }

    #endregion

  }

}
