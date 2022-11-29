using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using LibSaber.IO;
using LibSaber.Serialization;

namespace LibSaber.Shared.Structures
{

  public class BitSet<TCount> : IEnumerable<bool>, ISerialData<BitSet<TCount>>
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

    #region Public Methods

    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public bool HasFlag<TEnum>( TEnum flag )
      where TEnum : Enum
      => _bitArray[ GetFlagIndex( flag ) ];

    #endregion

    #region Private Methods

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private static int GetFlagIndex<TEnum>( TEnum value )
      where TEnum : Enum
    {
      var numericValue = Convert.ToUInt64( value );
      return BitOperations.Log2( numericValue );
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

    #region IEnumerable Methods

    public IEnumerator<bool> GetEnumerator()
    {
      for ( var i = 0; i < _count; i++ )
        yield return _bitArray[ i ];
    }

    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();

    #endregion

  }

}
