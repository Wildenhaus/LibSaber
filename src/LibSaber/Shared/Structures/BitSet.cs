using System.Collections;
using LibSaber.IO;
using LibSaber.Serialization;

namespace LibSaber.Shared.Structures
{

  public class BitSet<TCount> : ISerialData<BitSet<TCount>>
    where TCount : unmanaged, IConvertible
  {

    #region Data Members

    internal int _count;
    internal BitArray _bitArray;

    #endregion

    #region Properties

    public bool this[ int index ]
    {
      get => _bitArray[ index ];
    }

    #endregion

    #region Constructor

    public BitSet( int count, byte[] data )
    {
      _count = count;
      _bitArray = new BitArray( data );
    }

    internal BitSet( int count, BitArray bitArray )
    {
      _count = count;
      _bitArray = bitArray;
    }

    #endregion

    #region Serialization

    public static BitSet<TCount> Deserialize( NativeReader reader, ISerializationContext context )
    {
      var count = ReadCount(reader).ToInt32(null);

      const int FLAGS_PER_BYTE = 8;
      var numBytes = (count + 7) / FLAGS_PER_BYTE;

      var data = new byte[numBytes];
      reader.Read( data );

      return new BitSet<TCount>( count, data );
    }

    private static TCount ReadCount( NativeReader reader )
      => reader.ReadUnmanaged<TCount>();

    #endregion

  }

}
