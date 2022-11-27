using LibSaber.IO;

namespace LibSaber.Serialization
{

  public interface ISerialData<T>
  {

    public static abstract T Deserialize( NativeReader reader, ISerializationContext context );

  }

}
