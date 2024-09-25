using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibSaber.Shared.Structures;

namespace LibSaber.SpaceMarine2.Structures;

public abstract class m3dSPL
{
  /* This is the base class for splines.
   * Every type of spline is serialized the same, but the shape
   * of the data will be different based on the actual type.
   * 
   * Derived classes can implement the functionality to deliver
   * the appropriate data based on the internal spline data.
   */

  #region Data Members

  /// <summary>
  ///   The serial spline data.
  /// </summary>
  protected readonly SplineData _data;

  #endregion

  #region Properties

  /// <summary>
  ///   The spline type.
  /// </summary>
  public abstract SplineType Type { get; }

  /// <summary>
  ///   The number of elements in the spline.
  /// </summary>
  public uint Count
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => _data.Count;
  }

  #endregion

  #region Constructor

  /// <summary>
  ///   Constructs a new <see cref="m3dSPL" />.
  /// </summary>
  /// <param name="data">
  ///   The spline data.
  /// </param>
  protected m3dSPL(SplineData data)
  {
    ASSERT(data.SplineType == Type,
      "Provided SplineData ({0}) is not of type {1}.",
      data.SplineType, Type);

    _data = data;
  }

  #endregion

}

/// <summary>
///   A Linear 1D Spline structure.
/// </summary>
public sealed class m3dSPL_Linear1D : m3dSPL
{

  // TODO: Implement data accessors.

  #region Properties

  /// <inheritdoc cref="m3dSPL.Type" />
  public override SplineType Type => SplineType.Linear1D;

  #endregion

  #region Constructor

  public m3dSPL_Linear1D(SplineData splineData)
    : base(splineData)
  {
  }

  #endregion

}

/// <summary>
///   A Linear 2D Spline structure.
/// </summary>
public class m3dSPL_Linear2D : m3dSPL
{

  // TODO: Implement data accessors.

  #region Properties

  /// <inheritdoc cref="m3dSPL.Type" />
  public override SplineType Type => SplineType.Linear2D;

  #endregion

  #region Constructor

  public m3dSPL_Linear2D(SplineData splineData)
    : base(splineData)
  {
  }

  #endregion

}

/// <summary>
///   A Linear 3D Spline structure.
/// </summary>
public class m3dSPL_Linear3D : m3dSPL
{

  // TODO: Implement data accessors.

  #region Properties

  /// <inheritdoc cref="m3dSPL.Type" />
  public override SplineType Type => SplineType.Linear3D;

  #endregion

  #region Constructor

  public m3dSPL_Linear3D(SplineData splineData)
    : base(splineData)
  {
  }

  #endregion

}

/// <summary>
///   A Hermit Spline structure.
/// </summary>
public class m3dSPL_Hermit : m3dSPL
{

  // TODO: Implement data accessors.

  #region Properties

  /// <inheritdoc cref="m3dSPL.Type" />
  public override SplineType Type => SplineType.Hermit;

  #endregion

  #region Constructor

  public m3dSPL_Hermit(SplineData splineData)
    : base(splineData)
  {
  }

  #endregion

}

/// <summary>
///   A 2D Bezier Spline structure.
/// </summary>
public class m3dSPL_Bezier2D : m3dSPL
{

  // TODO: Implement data accessors.

  #region Properties

  /// <inheritdoc cref="m3dSPL.Type" />
  public override SplineType Type => SplineType.Bezier2D;

  #endregion

  #region Constructor

  public m3dSPL_Bezier2D(SplineData splineData)
    : base(splineData)
  {
  }

  #endregion

}

/// <summary>
///   A 3D Bezier Spline structure.
/// </summary>
public class m3dSPL_Bezier3D : m3dSPL
{

  #region Properties

  // TODO: Implement data accessors.

  /// <inheritdoc cref="m3dSPL.Type" />
  public override SplineType Type => SplineType.Bezier3D;

  #endregion

  #region Constructor

  public m3dSPL_Bezier3D(SplineData splineData)
    : base(splineData)
  {
  }

  #endregion

}

/// <summary>
///   A Lagrange Spline structure.
/// </summary>
public class m3dSPL_Lagrange : m3dSPL
{

  // TODO: Implement data accessors.

  #region Properties

  /// <inheritdoc cref="m3dSPL.Type" />
  public override SplineType Type => SplineType.Lagrange;

  #endregion

  #region Constructor

  public m3dSPL_Lagrange(SplineData splineData)
    : base(splineData)
  {
  }

  #endregion

}

/// <summary>
///   A Quaternarion Spline structure.
/// </summary>
public class m3dSPL_Quat : m3dSPL
{

  // TODO: Implement data accessors.

  #region Properties

  /// <inheritdoc cref="m3dSPL.Type" />
  public override SplineType Type => SplineType.Quat;

  #endregion

  #region Constructor

  public m3dSPL_Quat(SplineData splineData)
    : base(splineData)
  {
  }

  #endregion

}

/// <summary>
///   A Color Spline structure.
/// </summary>
public class m3dSPL_Color : m3dSPL
{

  // TODO: Implement data accessors.

  #region Properties

  /// <inheritdoc cref="m3dSPL.Type" />
  public override SplineType Type => SplineType.Color;

  #endregion

  #region Constructor

  public m3dSPL_Color(SplineData splineData)
    : base(splineData)
  {
  }

  #endregion

}

public struct SplineData
{

  #region Data Members

  public SplineType SplineType;
  public byte CompressedDataSize;
  public byte Unk_02;
  public byte Unk_03;
  public uint Count;
  public uint SizeInBytes;
  public float[] Data;

  #endregion

}
