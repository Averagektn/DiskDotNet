using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Disk.Visual.Impl;

/// <summary>
///     Represents a user with a center point, radius, speed, color, and initial size
/// </summary>
/// <param name="center">
///     The center point of the user
/// </param>
/// <param name="radius">
///     The radius of the user
/// </param>
/// <param name="speed">
///     The speed of the user
/// </param>
/// <param name="color">
///     The color of the user
/// </param>
/// <param name="parent">
///     Canvas, containing all figures
/// </param>
/// <param name="iniSize">
///     The initial size of the user
/// </param>
public class User(Point2D<int> center, int radius, int speed, Brush color, Canvas parent, Size iniSize) :
    Circle(center, radius, speed, color, parent, iniSize), IUser
{
    /// <inheritdoc/>
    public event Action<Point2D<int>>? OnShot;

    /// <inheritdoc/>
    public void ClearOnShot()
    {
        OnShot = null;
    }

    /// <inheritdoc/>
    public Point2D<int> Shot()
    {
        OnShot?.Invoke(Center);

        return Center;
    }
}
