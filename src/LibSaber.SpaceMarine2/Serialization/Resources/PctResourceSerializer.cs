using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Structures.Resources;
using YamlDotNet.Serialization;

namespace LibSaber.SpaceMarine2.Serialization.Resources;

public class PctResourceSerializer : SM2SerializerBase<resDESC_PCT>
{

  public override resDESC_PCT Deserialize(NativeReader reader, ISerializationContext context)
  {
    var deserializer = new DeserializerBuilder()
      .IgnoreUnmatchedProperties()
      .Build();

    using var streamReader = new StreamReader(reader.BaseStream, leaveOpen: true);
    var yaml = streamReader.ReadToEnd();

    var pctResource = deserializer.Deserialize<resDESC_PCT>(yaml);
    return pctResource;
  }

}
