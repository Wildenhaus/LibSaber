using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class scnSCENESerializer : SM2SerializerBase<scnSCENE>
{

  #region Constants

  private const uint SIGNATURE_1SER = 0x52455331; //1SER
  private const uint SIGNATURE_LG = 0x0000676C; //LG.

  private const uint SIGNATURE_SCN1 = 0x314E4353; //SCN1

  #endregion

  public override scnSCENE Deserialize(NativeReader reader, ISerializationContext context)
  {
    var scene = new scnSCENE();

    ReadSerLgHeader(reader);

    ReadSignature(reader, SIGNATURE_SCN1);
    ReadPropertyCount(reader, scene);

    ReadTextureListProperty(reader, scene);
    ReadPsProperty(reader, scene);
    ReadInstMaterialInfoListProperty(reader, scene);
    ReadGeometryMngProperty(reader, scene, context);

    return scene;
  }

  private void ReadSerLgHeader(NativeReader reader)
  {
    ReadSignature(reader, SIGNATURE_1SER);
    ReadSignature(reader, SIGNATURE_LG);

    _ = reader.ReadUInt64(); // Unk count
    _ = reader.ReadUInt64(); // Unk count
    _ = reader.ReadUInt64(); // Unk size

    _ = reader.ReadInt32(); // Unk flags
    var guid = reader.ReadGuid();
    _ = reader.ReadInt32(); // Unk

    var stringCount = reader.ReadInt32(); // Unk
    _ = reader.ReadInt32(); // Unk

    for (var i = 0; i < stringCount; i++)
    {
      _ = reader.ReadUInt16(); // Unk
      _ = reader.ReadByte(); // Unk

      _ = reader.ReadByte(); // Delimiter?
      _ = reader.ReadInt64(); // Unk
      _ = reader.ReadInt64(); // Unk
      _ = reader.ReadLengthPrefixedString32();
    }
  }

  private void ReadPropertyCount(NativeReader reader, scnSCENE scene)
  {
    /* These values are immediately after the SCN1 signature.
     * uint32 PropertyCount
     *   - Max index of the properties.
     * uint16 PropertyFlags
     *   - A BitField denoting which properties are present.
     */

    scene.PropertyCount = reader.ReadUInt32();
    scene.PropertyFlags = reader.ReadBitArray((int)scene.PropertyCount);
  }

  private void ReadTextureListProperty(NativeReader reader, scnSCENE scene)
  {
    /* A list of common textures that the Scene references.
     * Not sure how it's used.
     * Its a list of Pascal Strings (int16)
     */
    if (!scene.PropertyFlags[0])
      return;

    var count = reader.ReadUInt32();
    var unk0 = reader.ReadUInt16(); // TODO: Always 0?
    var endOffset = reader.ReadUInt32();

    scene.TextureList = new List<string>((int)count);
    for (var i = 0; i < count; i++)
      scene.TextureList.Add(reader.ReadLengthPrefixedString16());

    var endMarker = reader.ReadUInt16();
    endOffset = reader.ReadUInt32(); // EndOffset, points to next data
    ASSERT(endMarker == 0xFFFF, "Invalid EndMarker for TexList Property.");
    ASSERT(reader.Position == endOffset, "Invalid EndOffset for TexList Property.");
  }

  private void ReadPsProperty(NativeReader reader, scnSCENE scene)
  {
    /* Some sort of scripting tied to the Scene.
     * Most of the time this is just a one-line script denoting a base type.
     * RTTI says it uses a special serializer.
     * Represented as a Pascal String (int32)
     */
    if (!scene.PropertyFlags[1])
      return;

    scene.PS = reader.ReadLengthPrefixedString32();
  }

  private void ReadInstMaterialInfoListProperty(NativeReader reader, scnSCENE scene)
  {
    /* This seems to be some sort of scripting that can set custom properties on each
     * of the scene's TPLs or materials.
     * The first int32 is the count.
     * There is an unknown byte after that.
     * Then it's a list of Tuple<string,string>:
     *    Name: PascalString32 (can be empty)
     *    PropertyLine: PascalString32
     */
    if (!scene.PropertyFlags[2])
      return;

    var count = reader.ReadUInt32();
    var unk_01 = reader.ReadByte(); // TODO

    var list = scene.InstMaterialInfoList = new List<string>();
    for (var i = 0; i < count; i++)
    {
      var materialName = reader.ReadLengthPrefixedString32();
      var propertyLine = reader.ReadLengthPrefixedString32();

      var entry = string.Format("{0}: {1}", materialName, propertyLine);
      list.Add(entry);
    }
  }

  private void ReadGeometryMngProperty(NativeReader reader, scnSCENE scene, ISerializationContext context)
  {
    /* Geometry (Multi-Node Graph?) Data
     * Contains most of the model info.
     */
    scene.GeometryGraph = Serializer<objGEOM_MNG>.Deserialize(reader, context);
  }


}
