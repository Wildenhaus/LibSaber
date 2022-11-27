using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSaber.HaloCEA.Structures
{

  public struct Face
  {

    #region Data Members

    public short A;
    public short B;
    public short C;

    #endregion

    #region Constructor

    public Face()
    {
    }

    public Face( short a, short b, short c )
    {
      A = a;
      B = b;
      C = c;
    }

    #endregion

  }

}
