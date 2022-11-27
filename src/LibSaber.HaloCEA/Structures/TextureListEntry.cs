using LibSaber.IO;
using LibSaber.Serialization;

namespace LibSaber.HaloCEA.Structures
{

  public struct TextureListEntry : ISerialData<TextureListEntry>
  {

    #region Data Members

    public string TextureName;

    #endregion

    #region Casts

    public static implicit operator string( TextureListEntry entry )
      => entry.TextureName;

    public static implicit operator TextureListEntry( string textureName )
      => new TextureListEntry { TextureName = textureName };

    #endregion

    #region Serialization

    public static TextureListEntry Deserialize( NativeReader reader, ISerializationContext context )
    {
      return reader.ReadNullTerminatedString();
    }

    #endregion

  }

}
