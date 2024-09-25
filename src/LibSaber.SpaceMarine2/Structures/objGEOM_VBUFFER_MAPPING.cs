using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSaber.SpaceMarine2.Structures;

public class objGEOM_VBUFFER_MAPPING
{

  public List<objGEOM_STREAM_TO_VBUFFER> streamToVBuffer { get; set; }
  public List<objGEOM_VBUFFER_INFO> vBufferInfo { get; set; }

}
