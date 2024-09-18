using System.Reflection;
using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Structures.Resources;
using YamlDotNet.Serialization;

namespace LibSaber.SpaceMarine2.Serialization.Resources;

public class resDESCSerializer : SM2SerializerBase<resDESC>
{

  private static readonly Dictionary<string, Type> _typeMappings;
  private static readonly IDeserializer _deserializer;

  static resDESCSerializer()
  {
    _typeMappings = InitTypeMappings();

    _deserializer = new DeserializerBuilder()
      .IgnoreUnmatchedProperties()
      .WithTypeDiscriminatingNodeDeserializer(o =>
      {
        o.AddKeyValueTypeDiscriminator<resDESC>("__type", _typeMappings);
      })
      .Build();
  }

  private static Dictionary<string, Type> InitTypeMappings()
  {
    var types = typeof(resDESC).Assembly.GetTypes()
      .Where(x => x.IsAssignableTo(typeof(resDESC)))
      .Where(x => x.IsClass && !x.IsAbstract);

    var mappings = new Dictionary<string, Type>();
    foreach (var type in types)
    {
      var typeNameAttribute = type.GetCustomAttribute<ResDESCTypeDiscriminatorAttribute>();
      if (typeNameAttribute is null)
        continue;

      mappings.Add(typeNameAttribute.TypeName, type);
    }

    return mappings;
  }

  public override resDESC Deserialize(NativeReader reader, ISerializationContext context)
  {
    using var textReader = new StreamReader(reader.BaseStream, leaveOpen: true);

    return _deserializer.Deserialize<resDESC>(textReader);
  }

}
