using System.Diagnostics;
using LibSaber.IO;
using LibSaber.Shared.Structures;

namespace LibSaber.Serialization
{

  public class SentinelReader<TOffset>
    where TOffset : unmanaged, IConvertible
  {

    #region Data Members

    private readonly Stream _stream;
    private readonly NativeReader _reader;

    private int _index;
    private Sentinel<TOffset> _currentSentinel;

    #endregion

    #region Properties

    public Sentinel<TOffset> Sentinel
    {
      get => _currentSentinel;
    }

    public short SentinelId
    {
      get => _currentSentinel.Id;
    }

    public long EndOffset
    {
      get => _currentSentinel.EndOffset;
    }

    #endregion

    #region Constructor

    public SentinelReader( NativeReader reader )
    {
      _stream = reader.BaseStream;
      _reader = reader;

      _index = -1;
      _currentSentinel = default;
    }

    #endregion

    #region Public Methods

    public bool Next( bool boundsCheck = true )
    {
      if ( boundsCheck && _index > -1 )
      {
        if ( EndOffset < _reader.Position )
          FAIL( "Over-read Sentinel 0x{0:X2} block.", Sentinel.Id );
        else if ( EndOffset > _reader.Position )
          FAIL( "Under-read Sentinel 0x{0:X2} block.", Sentinel.Id );
      }

      _index++;
      _currentSentinel = Sentinel<TOffset>.Read( _reader );

      if ( _reader.Position == _stream.Length )
        return false;

      return true;
    }

    public void BurnSentinel()
    {
      Next();
    }

    [DebuggerHidden]
    public void ReportUnknownSentinel()
      => FAIL( "Unknown Sentinel: 0x{0:X2}", SentinelId );

    #endregion

  }

}
