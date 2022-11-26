using LibSaber.IO;

namespace LibSaber.Shared
{

  public readonly struct Sentinel<TSentinel, TOffset>
    where TSentinel : unmanaged
    where TOffset : unmanaged
  {

    #region Data Members

    public readonly TSentinel Id;
    public readonly TOffset EndOffset;

    #endregion

    #region Constructor

    public Sentinel( TSentinel id, TOffset endOffset )
    {
      Id = id;
      EndOffset = endOffset;
    }

    public static Sentinel<TSentinel, TOffset> Read( NativeReader reader )
    {
      var id = reader.ReadUnmanaged<TSentinel>();
      var endOffset = reader.ReadUnmanaged<TOffset>();

      return new Sentinel<TSentinel, TOffset>( id, endOffset );
    }

    #endregion

  }

}
