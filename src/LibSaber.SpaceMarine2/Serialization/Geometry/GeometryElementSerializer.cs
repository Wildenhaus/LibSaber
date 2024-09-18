using LibSaber.SpaceMarine2.Enumerations;
using LibSaber.SpaceMarine2.Structures;
using LibSaber.IO;
using LibSaber.Shared.Structures;

namespace LibSaber.SpaceMarine2.Serialization.Geometry
{

  public abstract class GeometryElementSerializer<T>
  {
    #region Properties

    protected GeometryBuffer Buffer { get; }
    protected GeometryBufferFlags Flags => Buffer.Flags;

    #endregion

    #region Constructor

    protected GeometryElementSerializer( GeometryBuffer buffer )
    {
      Buffer = buffer;
    }

    #endregion

    #region Private Methods

    public abstract T Deserialize( NativeReader reader );

    public abstract IEnumerable<T> DeserializeRange( NativeReader reader, int startIndex, int endIndex );

    #endregion

  }

}
