using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Structures;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class animROOTEDSerializer : SM2SerializerBase<animROOTED>
{

  public override animROOTED Deserialize(NativeReader reader, ISerializationContext context)
  {
    var anim = new animROOTED();
    var properties = BitSet<int>.Deserialize(reader, context);

    if (properties.HasFlag(AnimationRootedProperty.IniTranslation))
      anim.IniTranslation = reader.ReadVector3();

    if (properties.HasFlag(AnimationRootedProperty.PTranslation))
      anim.PTranslation = Serializer<m3dSPL>.Deserialize(reader, context);

    if (properties.HasFlag(AnimationRootedProperty.IniRotation))
      anim.IniRotation = reader.ReadVector4();

    if (properties.HasFlag(AnimationRootedProperty.PRotation))
      anim.PRotation = Serializer<m3dSPL>.Deserialize(reader, context);

    return anim;
  }

  #region Property Flags

  [Flags]
  private enum AnimationRootedProperty : byte
  {
    IniTranslation = 1 << 0,
    PTranslation = 1 << 1,
    IniRotation = 1 << 2,
    PRotation = 1 << 3
  }

  #endregion

}
