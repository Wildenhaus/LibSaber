using LibSaber.IO;
using LibSaber.Serialization;

namespace LibSaber.HaloCEA.Structures
{

  public class DataList<T> : List<T>, ISerialData<DataList<T>>
    where T : ISerialData<T>
  {

    #region Serialization

    public static DataList<T> Deserialize( NativeReader reader, ISerializationContext context )
    {
      var list = new DataList<T>();

      var count = reader.ReadInt32();
      for ( var i = 0; i < count; i++ )
        list.Add( T.Deserialize( reader, context ) );

      return list;
    }

    #endregion

  }
}
