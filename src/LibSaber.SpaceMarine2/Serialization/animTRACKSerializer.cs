using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.Shared.Structures;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class animTRACKSerializer : SM2SerializerBase<animTRACK>
{

  #region Overrides

  public override animTRACK Deserialize(NativeReader reader, ISerializationContext context)
  {
    var track = new animTRACK();
    var propertyFlags = BitSet<int>.Deserialize(reader, context);

    if (propertyFlags.HasFlag(AnimationTrackProperties.AnimationSequences))
      track.seqList = Serializer<List<animSEQ>>.Deserialize(reader, context);

    if (propertyFlags.HasFlag(AnimationTrackProperties.ObjectAnimations))
      track.objAnimList = Serializer<List<animOBJ_ANIM>>.Deserialize(reader, context);

    if (propertyFlags.HasFlag(AnimationTrackProperties.ObjectMap))
      FAIL("This property has never been observed as in-use, and as such is not implemented.");

    if (propertyFlags.HasFlag(AnimationTrackProperties.RootAnimation))
      track.rootAnim = Serializer<animROOTED>.Deserialize(reader, context);

    return track;
  }
  #endregion

  #region Property Flags

  private enum AnimationTrackProperties : byte
  {
    AnimationSequences = 1 << 0,
    ObjectAnimations = 1 << 1,
    ObjectMap = 1 << 2,
    RootAnimation = 1 << 3,
  }

  #endregion

}
