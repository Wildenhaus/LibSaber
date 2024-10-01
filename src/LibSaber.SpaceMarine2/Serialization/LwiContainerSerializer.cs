using System.Numerics;
using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class LwiContainerSerializer : SM2SerializerBase<LwiContainer>
{

  public override LwiContainer Deserialize(NativeReader reader, ISerializationContext context)
  {
    var container = new LwiContainer();

    // Reversed by RED_EYE
    ReadHeader(reader, container);

    var count = reader.ReadInt32();
    for (var i = 0; i < count; i++)
      container.Add(new());

    ReadPrimaryData(reader, container);
    ReadInstanceData(reader, container);

    return container;
  }

  private void ReadHeader(NativeReader reader, LwiContainer container)
  {
    var signature = reader.ReadFixedLengthString(0x11);
    ASSERT(signature == "1SERlwi_container");
    reader.Position += 0x13; // Padding

    ASSERT(reader.ReadFixedLengthString(0x10) == "S3DRESOURCE     ");

    var toolVersion = reader.ReadUInt32();
    var linksCount = reader.ReadUInt32();
    var linkListSize = reader.ReadUInt32();

    var unk_01 = reader.ReadUInt32();
    container.Version = reader.ReadUInt32();
    var containerType = reader.ReadLengthPrefixedString32();

    ASSERT(containerType == "lwi_container_static",
      $"Unsupported LWI Container Type: {containerType}");

    var unk_02 = reader.ReadLengthPrefixedString32();
  }

  private void ReadPrimaryData(NativeReader reader, LwiContainer container)
  {
    var version = container.Version;
    foreach(var element in container)
    {
      element.Name = reader.ReadLengthPrefixedString32();
      if (version >= 8)
      {
        element.Type = reader.ReadLengthPrefixedString32();
        element.CreateCollisionActor = reader.ReadByte();
        element.TplName = reader.ReadLengthPrefixedString32();

        var count = reader.ReadUInt32();
        element.Archetypes = new List<LwiContainer.Element.Archetype>();
        for(var i = 0; i < count; i++)
        {
          element.Archetypes.Add(new LwiContainer.Element.Archetype
          {
            TextureName = reader.ReadLengthPrefixedString32(),
            SourceMaterialName = reader.ReadLengthPrefixedString32(),
            TargetMaterialName = reader.ReadLengthPrefixedString32(),
            ObjectName = reader.ReadLengthPrefixedString32(),
            Name = reader.ReadByte()
          });
        }
      }

      if (version > 8)
      {
        element.__type = reader.ReadInt64();
      }
    }
  }

  private void ReadInstanceData(NativeReader reader, LwiContainer container)
  {
    var version = container.Version;

    var count = reader.ReadUInt32();
    ASSERT(count == container.Count);

    foreach(var element in container)
    {
      element.ElementId = reader.ReadUInt32();
      if(version >= 7) element.HashedMaterialOverrides = reader.ReadUInt32();
      
      var instanceCount = reader.ReadUInt32();
      for(var i = 0; i < instanceCount; i++)
      {
        var instance = new LwiContainer.Element.Instance();
        element.Add(instance);

        instance.Mat = ReadLwiSerializableTransform(reader);

        if(version >= 6)
        {
          var matCount = reader.ReadUInt32();
          instance.UnkMatData = new (ushort, System.Numerics.Matrix4x4)[matCount];
          for(var j = 0; j < matCount; j++)
            instance.UnkMatData[j] = (reader.ReadUInt16(), reader.ReadMatrix4x4());
        }

        if(version >= 10)
        {
          instance.MaterialIndex = reader.ReadByte();
          if(version >= 12)
          {
            instance.UserInNavmesh = reader.ReadByte();
            instance.Unk = reader.ReadByte();
          }
        }
      }
    }
  }

  private Matrix4x4 ReadLwiSerializableTransform(NativeReader reader)
  {
    var vec_3 = reader.ReadVector3();
    var vec_0 = reader.ReadVector3();
    var vec_1 = reader.ReadVector3();
    var vec_2 = reader.ReadVector3();

    var matr = Matrix4x4.Identity;
    matr = new Matrix4x4(
      vec_0.X, vec_0.Y, vec_0.Z, matr.M14,
      vec_1.X, vec_1.Y, vec_1.Z, matr.M24,
      vec_2.X, vec_2.Y, vec_2.Z, matr.M34,
      vec_3.X, vec_3.Y, vec_3.Z, matr.M44
      );

    return matr;
  }

}
