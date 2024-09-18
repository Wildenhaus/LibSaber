using LibSaber.SpaceMarine2.Enumerations;

namespace LibSaber.SpaceMarine2.Structures
{
    public class pctPICTURE
    {

      #region Properties

      public int Width { get; set; }
      public int Height { get; set; }
      public int Depth { get; set; }
      public int Faces { get; set; }
      public int MipMapCount { get; set; }
      public SM2TextureFormat Format { get; set; }

      public byte[] Data { get; set; }

      #endregion

    }
}
