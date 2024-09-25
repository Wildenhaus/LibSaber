using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Serialization.Scripting;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class cdLISTSerializer : SM2SerializerBase<cdLIST>
{

  #region Overrides

  public override cdLIST Deserialize(NativeReader reader, ISerializationContext context)
  {
    ReadHeader(reader);

    var count = reader.ReadInt32();
    var unk = reader.ReadInt32();

    var list = new cdLIST(count);
    for (var i = 0; i < count; i++)
      list.Add(new());

    ReadNames(reader, list);
    ASSERT(reader.ReadByte() == 0);
    ReadMatrices(reader, list);
    ReadAffixes(reader, list);
    ReadPsSection(reader, list);

    ParseTypes(list);
    ParseTpls(list);
    return list;
  }

  #endregion

  #region Private Methods

  private void ReadHeader(NativeReader reader)
  {
    const string SIGNATURE_CD_LIST = "1SERcd_list";

    var signature = reader.ReadFixedLengthString(SIGNATURE_CD_LIST.Length);

    ASSERT(signature.StartsWith(SIGNATURE_CD_LIST),
      "Invalid signature for CdList.");

    reader.Position += 0x35;
  }

  private void ReadNames(NativeReader reader, cdLIST list)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var entry in list)
      entry.Name = reader.ReadLengthPrefixedString32();
  }

  private void ReadMatrices(NativeReader reader, cdLIST list)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var entry in list)
      entry.Matrix = reader.ReadMatrix4x4();
  }

  private void ReadAffixes(NativeReader reader, cdLIST list)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var entry in list)
      entry.Affixes = reader.ReadLengthPrefixedString32();
  }

  private void ReadPsSection(NativeReader reader, cdLIST list)
  {
    if (reader.ReadByte() == 0)
      return;

    var flags = reader.ReadBitArray(list.Count);
    var scriptSerializer = new PsObjectTextSerializer();

    for (var i = 0; i < list.Count; i++)
    {
      if (!flags[i])
        continue;

      var entry = list[i];
      var endOfBlock = false;

      var sentinelReader = new SentinelReader<int>(reader);
      while (sentinelReader.Next())
      {
        switch ((ushort)sentinelReader.SentinelId)
        {
          case 0x0000:
            var script = entry.Script = reader.ReadLengthPrefixedString32();
            entry.ScriptData = scriptSerializer.Deserialize(script);
            break;

          case 0xFFFF:
            endOfBlock = true;
            break;

          default:
            sentinelReader.ReportUnknownSentinel();
            break;
        }

        if (endOfBlock)
          break;
      }
    }
  }

  private void ParseTypes(cdLIST list)
  {
    foreach (var entry in list)
    {
      if (entry.ScriptData is null)
        continue;

      if (!entry.ScriptData.TryGetValue("__type", out var type))
        continue;

      entry.__type = type.ToString();
    }
  }

  private void ParseTpls(cdLIST list)
  {
    foreach(var entry in list)
    {
      entry.NameTpls = new List<string>();

      if (entry.ScriptData is null)
        continue;

      if (!entry.ScriptData.TryGetValue("properties", out dynamic properties))
        continue;

      if (!properties.TryGetValue("prop_scene_container", out dynamic prop_scene_container))
        continue;

      if (!prop_scene_container.TryGetValue("staticActorDescs", out dynamic staticActorDescs))
        continue;

      foreach(var staticDesc in staticActorDescs)
      {
        if (!staticDesc.TryGetValue("desc", out dynamic desc))
          continue;

        if (!desc.TryGetValue("properties", out dynamic descProps))
          continue;

        if (!descProps.TryGetValue("geom", out dynamic descGeom))
          continue;

        if (!descGeom.TryGetValue("nameTpl", out dynamic nameTpl))
          continue;

        entry.NameTpls.Add(nameTpl.ToString());
      }
    }
  }

  #endregion

}
