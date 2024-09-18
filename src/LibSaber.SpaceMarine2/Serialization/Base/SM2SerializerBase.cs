using LibSaber.IO;
using LibSaber.Serialization;

namespace LibSaber.SpaceMarine2.Serialization;

public abstract class SM2SerializerBase<T>
{

  #region Public Methods

  public abstract T Deserialize(NativeReader reader, ISerializationContext context);

  #endregion

  #region Helper Methods

  protected uint ReadSignature(NativeReader reader, uint expectedSignature)
  {
    var actualSignature = reader.ReadUInt32();
    ASSERT(actualSignature == expectedSignature,
      "The signature that was read ({0:X}) does not match the signature " +
      "that was provided ({1:X}).",
      actualSignature, expectedSignature);

    return actualSignature;
  }

  #endregion

}
