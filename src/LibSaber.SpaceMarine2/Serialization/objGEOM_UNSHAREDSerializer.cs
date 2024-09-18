using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class objGEOM_UNSHAREDSerializer : SM2SerializerBase<List<objGEOM_UNSHARED>>
{

  public override List<objGEOM_UNSHARED> Deserialize(NativeReader reader, ISerializationContext context)
  {
    var objects = context.GetMostRecentObject<List<objOBJ>>();
    var objectCount = objects.Count;

    var geoms = new List<objGEOM_UNSHARED>();
    var objFlags = reader.ReadBitArray(objectCount);

    for(var i = 0; i < objectCount; i++)
    {
      if (!objFlags[i])
      {
        geoms.Add(null);
        continue;
      }

      geoms.Add(ReadGeomUnshared(reader));
    }

    return geoms;
  }

  private objGEOM_UNSHARED ReadGeomUnshared(NativeReader reader)
  {
    var geom = new objGEOM_UNSHARED();

    var propertyCount = reader.ReadInt32();
    var propertyFlags = (PropertyFlags)reader.ReadByte();

    if (propertyFlags.HasFlag(PropertyFlags.SplitIndex))
      geom.splitIndex = reader.ReadUInt32();
    if (propertyFlags.HasFlag(PropertyFlags.NumSplits))
      geom.numSplits = reader.ReadUInt32();
    if(propertyFlags.HasFlag(PropertyFlags.BoundingBox))
      geom.bbox = new m3dBOX(reader.ReadVector3(), reader.ReadVector3());
    if(propertyFlags.HasFlag(PropertyFlags.Obb))
      geom.obb = m3dOBB.Deserialize(reader);

    return geom;
  }

  enum PropertyFlags : Byte
  {
    SplitIndex = 1 << 0,
    NumSplits = 1 << 1,
    BoundingBox = 1 << 2,
    Obb = 1 << 3
  }

}
