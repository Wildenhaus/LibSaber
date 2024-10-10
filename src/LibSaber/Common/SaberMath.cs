using System.Numerics;
using System.Runtime.CompilerServices;

namespace LibSaber.Common
{

  public static class SaberMath
  {

    /// <summary>
    ///   Returns the fractional part of each float in the vector.
    /// </summary>
    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
    public static Vector2 Frac( in Vector2 value )
    {
      return new Vector2(
        x: value.X % 1.0f,
        y: value.Y % 1.0f
        );
    }

    /// <summary>
    ///   Returns the fractional part of each float in the vector.
    /// </summary>
    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
    public static Vector3 Frac( in Vector3 value )
    {
      return new Vector3(
        x: value.X % 1.0f,
        y: value.Y % 1.0f,
        z: value.Z % 1.0f
        );
    }

    /// <summary>
    ///   Returns the value, normalized to a range of [0,1].
    /// </summary>
    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
    public static float Saturate( in float value )
      => Math.Min( 1.0f, Math.Max( 0.0f, value ) );

    /// <summary>
    ///   Returns the sign of the input value.
    /// </summary>
    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
    public static short Sign( in short value )
      => ( short ) ( value < 0 ? -1 : 1 );

    /// <summary>
    ///   Unpacks a <see cref="Vector3{float}" /> from an Int16 value.
    /// </summary>
    /// <param name="w">
    ///   The Int16 value to unpack.
    /// </param>
    /// <returns>
    ///   The unpacked vector.
    /// </returns>
    [MethodImpl(Globals.METHOD_IMPL_AGGRESSIVE)]
    public static Vector3 UnpackVector3FromInt16( short w )
    {
      /* Shader code works as follows:
       *  xz  = (-1.f + 2.f * frac( float2(1.f/181, 1.f/181.0/181.0) * abs(w))) * float2(181.f/179.f, 181.f/180.f);
       *  y   = sign(inInt16Value) * sqrt(saturate(1.f - tmp.x*tmp.x - tmp.z*tmp.z));
       */

      var negativeIdentity = new Vector2( -1.0f );

      var xz = ( negativeIdentity + 2.0f * Frac( new Vector2( 1.0f / 181, 1.0f / 181.0f / 181.0f ) * Math.Abs( w ) ) );
      xz *= new Vector2( 181.0f / 179.0f, 181.0f / 180.0f );

      var yTmp = Sign( w ) * Math.Sqrt( Saturate( 1.0f - xz.X * xz.X - xz.Y * xz.Y ) );

      var x = ( float ) xz.X;
      var y = ( float ) yTmp;
      var z = ( float ) xz.Y;

      return new Vector3( x, y, z );
    }

  }

}
