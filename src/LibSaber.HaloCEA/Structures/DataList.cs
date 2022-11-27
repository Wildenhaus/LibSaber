using LibSaber.IO;
using LibSaber.Serialization;

namespace LibSaber.HaloCEA.Structures
{

  public struct DataList<T> : ISerialData<DataList<T>>
    where T : ISerialData<T>
  {

    #region Data Members

    public int Count;
    public List<T> Entries;

    #endregion

    #region Casts

    public static implicit operator List<T>( DataList<T> dataList )
      => dataList.Entries;

    #endregion

    #region Serialization

    public static DataList<T> Deserialize( NativeReader reader, ISerializationContext context )
    {
      var list = new DataList<T>();

      var count = list.Count = reader.ReadInt32();

      var entries = list.Entries = new List<T>( count );
      for ( var i = 0; i < count; i++ )
        entries.Add( T.Deserialize( reader, context ) );

      return list;
    }

    #endregion

  }
}
