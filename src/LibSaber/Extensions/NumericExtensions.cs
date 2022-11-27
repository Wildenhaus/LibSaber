using LibSaber.Shared.Structures;

namespace LibSaber.Extensions
{

  public static class NumericExtensions
  {

    public static float SNormToFloat( this short snormValue )
      => snormValue / SNorm16.Coefficient;

  }

}
