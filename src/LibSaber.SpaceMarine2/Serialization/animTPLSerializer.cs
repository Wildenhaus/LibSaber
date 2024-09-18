using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Structures;
using LibSaber.SpaceMarine2.Enumerations;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class animTPLSerializer : SM2SerializerBase<animTPL>
{

  #region Constants

  private const uint SIGNATURE_1SER = 0x52455331; //1SER
  private const uint SIGNATURE_TPL = 0x006C7074; //TPL.

  private const uint SIGNATURE_TPL1 = 0x314C5054; //TPL1

  #endregion

  public override animTPL Deserialize(NativeReader reader, ISerializationContext context)
  {
    var template = new animTPL();

    ReadSerTplHeader(reader);
    ReadSignature(reader, SIGNATURE_TPL1);

    var propertyFlags = BitSet<int>.Deserialize(reader, context);

    if (propertyFlags.HasFlag(TemplatePropertyFlags.Name))
      ReadNameProperty(reader, template);
    if (propertyFlags.HasFlag(TemplatePropertyFlags.NameClass))
      ReadNameClassProperty(reader, template);
    if (propertyFlags.HasFlag(TemplatePropertyFlags.State))
      ReadStateProperty(reader, template, context);
    if (propertyFlags.HasFlag(TemplatePropertyFlags.Affixes))
      ReadAffixesProperty(reader, template);
    if (propertyFlags.HasFlag(TemplatePropertyFlags.PS))
      ReadPsProperty(reader, template);
    if (propertyFlags.HasFlag(TemplatePropertyFlags.Skin))
      ReadSkinProperty(reader, template);
    if (propertyFlags.HasFlag(TemplatePropertyFlags.TrackAnim))
      ReadTrackAnimProperty(reader, template, context);
    if (propertyFlags.HasFlag(TemplatePropertyFlags.BoundingBox))
      ReadBoundingBoxProperty(reader, template);
    if (propertyFlags.HasFlag(TemplatePropertyFlags.LodDefinition))
      ReadLodDefinitionProperty(reader, template, context);
    if (propertyFlags.HasFlag(TemplatePropertyFlags.TextureList))
      ReadTexListProperty(reader, template);
    if (propertyFlags.HasFlag(TemplatePropertyFlags.GeometryMNG))
      ReadGeometryMngProperty(reader, template, context);

    return template;
  }

  #region Private Methods

  private void ReadSerTplHeader(NativeReader reader)
  {
    ReadSignature(reader, SIGNATURE_1SER);
    ReadSignature(reader, SIGNATURE_TPL);

    var unk_0 = reader.ReadUInt64(); // Unk count
    var unk_a = reader.ReadUInt64(); // Unk count
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

  private void ReadNameProperty(NativeReader reader, animTPL template)
  {
    /* This is the name of the Template.
     * LengthPrefixed String (int32)
     */
    template.name = reader.ReadLengthPrefixedString32();
  }

  private void ReadNameClassProperty(NativeReader reader, animTPL template)
  {
    /* Not sure what this is. Haven't encountered any files with it yet.
     * RTTI states that it's a LengthPrefixed String (int32)
     */
    template.nameClass = reader.ReadLengthPrefixedString32();
  }

  private void ReadStateProperty(NativeReader reader, animTPL template, ISerializationContext context)
  {
    /* State flags
     * TODO: Figure out what these are
     */
    var bitCount = reader.ReadUInt16();
    template.state = (TPLState)reader.ReadUInt32();
    //Console.Write("{0} {1} ", bitCount, template.state.ToString());
  }

  private void ReadAffixesProperty(NativeReader reader, animTPL template)
  {
    // TODO
    /* A bunch of export/attribute strings. Not sure what they're used for.
     * RTTI says this uses a special string serializer.
     * There seems to be a delimiter between each string. It might be deserialized to a list.
     * Represented as a LengthPrefixed String (int32)
     */
    template.affixes = reader.ReadLengthPrefixedString32();
  }

  private void ReadPsProperty(NativeReader reader, animTPL template)
  {
    /* Some sort of scripting tied to the Template.
     * Most of the time this is just a one-line script denoting a base type.
     * RTTI says it uses a special serializer.
     * Represented as a LengthPrefixed String (int32)
     */
    template.ps = reader.ReadLengthPrefixedString32();
  }

  private void ReadSkinProperty(NativeReader reader, animTPL template)
  {
    template.skin = Serializer<tplSKIN>.Deserialize(reader);
  }

  private void ReadTrackAnimProperty(NativeReader reader, animTPL template, ISerializationContext context)
  {
    /* Animation Tracks for the Template.
     */
    template.trackAnim = Serializer<animTRACK>.Deserialize(reader);
  }

  private void ReadBoundingBoxProperty(NativeReader reader, animTPL template)
  {
    /* Bounding box for the whole Template.
     */
    template.bbox = new m3dBOX(reader.ReadVector3(), reader.ReadVector3());
  }

  private void ReadLodDefinitionProperty(NativeReader reader, animTPL template, ISerializationContext context)
  {
    /* Level-of-detail definitions for the Template.
     */
    template.lodDef = Serializer<List<tplLOD_DEF>>.Deserialize(reader, context);
  }

  private void ReadTexListProperty(NativeReader reader, animTPL template)
  {
    /* A list of common textures that the Template references.
     * Its a list of LengthPrefixed Strings (int16)
     */
    var count = reader.ReadUInt32();
    var unk0 = reader.ReadUInt16(); // TODO: Always 0?
    var endOffset = reader.ReadUInt32();

    template.TexList = new List<string>((int)count);
    for (var i = 0; i < count; i++)
      template.TexList.Add(reader.ReadLengthPrefixedString16());

    var endMarker = reader.ReadUInt16();
    _ = reader.ReadUInt32(); // EndOffset, points to next data
    ASSERT(endMarker == 0xFFFF, "Invalid EndMarker for TexList Property.");
  }

  private void ReadGeometryMngProperty(NativeReader reader, animTPL template, ISerializationContext context)
  {
    /* Geometry (Multi-Node Graph?) Data
     * Contains most of the model info.
     */
    template.GeometryGraph = Serializer<objGEOM_MNG>.Deserialize(reader, context);
  }


  #endregion

  #region Property Flags

  [Flags]
  public enum TemplatePropertyFlags : ushort
  {
    Name = 1 << 0,
    NameClass = 1 << 1,
    State = 1 << 2,
    Affixes = 1 << 3,
    PS = 1 << 4,
    Skin = 1 << 5,
    TrackAnim = 1 << 6,
    OnReadAnimExtra = 1 << 7,
    BoundingBox = 1 << 8,
    LodDefinition = 1 << 9,
    TextureList = 1 << 10,
    GeometryMNG = 1 << 11,
    ExternData = 1 << 12
  }

  #endregion

}
