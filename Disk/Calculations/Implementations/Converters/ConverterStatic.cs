using Disk.Data.Impl;

namespace Disk.Calculations.Implementations.Converters;

/// <summary>
///     Provides conversion between coordinate systems
/// </summary>
public partial class Converter
{
    /// <summary>
    ///     Converts the angle from degree to radian
    /// </summary>
    /// <param name="angle">
    ///     The angle in degrees
    /// </param>
    /// <returns>
    ///     The converted angle in radian
    /// </returns>
    public static float ToRadian_FromAngle(float angle)
    {
        return (float)(angle * Math.PI / 180);
    }

    /// <summary>
    ///     Converts the 2D angle coordinate from degree to radian
    /// </summary>
    /// <param name="angle">
    ///     The 2D angle coordinate in degrees
    /// </param>
    /// <returns>
    ///     The converted 2D angle coordinate in radian
    /// </returns>
    public static Point2D<float> ToRadian_FromAngle(Point2D<float> angle)
    {
        return new(ToRadian_FromAngle(angle.X), ToRadian_FromAngle(angle.Y));
    }

    /// <summary>
    ///     Converts the 3D angle coordinate from degree to radian
    /// </summary>
    /// <param name="angle">
    ///     The 3D angle coordinate in degrees
    /// </param>
    /// <returns>
    ///     The converted 3D angle coordinate in radian
    /// </returns>
    public static Point3D<float> ToRadian_FromAngle(Point3D<float> angle)
    {
        return new(ToRadian_FromAngle(angle.X), ToRadian_FromAngle(angle.Y), ToRadian_FromAngle(angle.Z));
    }
}
