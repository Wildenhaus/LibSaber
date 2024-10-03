using LibSaber.IO;
using LibSaber.Serialization;
using LibSaber.SpaceMarine2.Structures;

namespace LibSaber.SpaceMarine2.Serialization;

public class tplSKINSerializer : SM2SerializerBase<tplSKIN>
{

  public override tplSKIN Deserialize(NativeReader reader, ISerializationContext context)
  {
    var skin = new tplSKIN();

    var nBones = skin.nBones = reader.ReadUInt32();
    var unk_0 = reader.ReadUInt16(); // TODO
    var unk_1 = reader.ReadByte(); // TODO

    while (true)
    {
      var sentinel = reader.ReadUInt16();
      var endOffset = reader.ReadUInt32();
      if (sentinel == 0xFFFF) // End of chunk
      {
        ASSERT(endOffset == reader.Position, "Invalid end position for TPL1 Skin property.");
        break;
      }


      if (sentinel == 0x00) // boneInvBindMatrList
      {
        var boneInvBindMatrList = skin.boneInvBindMatrList = new System.Numerics.Matrix4x4[nBones];
        for (var i = 0; i < nBones; i++)
          boneInvBindMatrList[i] = reader.ReadMatrix4x4();
      }
      else if (sentinel == 0x01) // lodBonesCount
      {
        var listCount = reader.ReadUInt32();
        var lodBonesCount = skin.lodBonesCount = new ushort[listCount];
        for (var i = 0; i < listCount; i++)
          lodBonesCount[i] = reader.ReadUInt16();
      }

      ASSERT(endOffset == reader.Position, "Invalid end position for TPL1 Skin property.");
    }

    return skin;
  }

}
