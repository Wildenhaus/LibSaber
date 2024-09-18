using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Serialization.Scripting;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class ClassListSerializer : SM2SerializerBase<ClassList>
{

  const string MAGIC_HEADER = "1SERclass_list";

  public override ClassList Deserialize(NativeReader reader, ISerializationContext context)
  {
    ReadHeader(reader);

    
    var count = reader.ReadInt32();
    var propCount = reader.ReadInt32();

    var classList = new ClassList();
    for (var i = 0; i < count; i++)
      classList.Add(new());

    if (propCount > 0)
      ReadNameProperty(reader, classList);
    if (propCount > 1)
      ReadPsProperty(reader, classList);

    BuildTplLookup(classList);
    return classList;
  }

  private void ReadHeader(NativeReader reader)
  {
    var magic = reader.ReadFixedLengthString(MAGIC_HEADER.Length);
    ASSERT(magic == MAGIC_HEADER, "Invalid ClassList magic.");

    reader.Position = 0x40;
  }

  private void ReadNameProperty(NativeReader reader, ClassList classList)
  {
    if (reader.ReadByte() == 0)
      return;

    foreach (var entry in classList)
      entry.Name = reader.ReadLengthPrefixedString32();
  }

  private void ReadPsProperty(NativeReader reader, ClassList classList)
  {
    if (reader.ReadByte() == 0)
      return;

    var psSerializer = new PsObjectTextSerializer();

    foreach (var entry in classList)
    {
      var script = reader.ReadLengthPrefixedString32();
      entry.PS = psSerializer.Deserialize(script);
    }
  }

  private void BuildTplLookup(ClassList classList)
  {
    var lookup = new Dictionary<string, string>();
    foreach (var entry in classList)
    {
      if (entry.PS is null)
        continue;

      if (!entry.PS.TryGetValue("properties", out dynamic properties))
        continue;

      if (!properties.TryGetValue("geom", out dynamic geom))
        continue;

      if (!geom.TryGetValue("nameTpl", out dynamic nameTpl))
        continue;

      lookup.Add(entry.Name, nameTpl.ToString());
    }

    classList.TplLookup = lookup;
  }

}
