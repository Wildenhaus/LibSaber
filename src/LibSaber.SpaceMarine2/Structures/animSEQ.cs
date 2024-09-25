namespace LibSaber.SpaceMarine2.Structures;

public class animSEQ
{

  public string name { get; set; }
  public uint layerId { get; set; }
  public float startFrame { get; set; }
  public float endFrame { get; set; }
  public float offsetFrame { get; set; }
  public float lenFrame { get; set; }
  public float timeSec { get; set; }
  public List<ActionFrame> actionFrames { get; set; }
  public m3dBOX bbox { get; set; } 

}
