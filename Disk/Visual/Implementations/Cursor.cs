using Disk.Data.Impl;
using Disk.Visual.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Disk.Visual.Implementations;

/// <summary>
///     Represents a cursor with a center point, radius, speed, color, and initial size
/// </summary>
/// <param name="center">
///     The center point of the cursor
/// </param>
/// <param name="radius">
///     The radius of the cursor
/// </param>
/// <param name="speed">
///     The speed of the cursor
/// </param>
/// <param name="color">
///     The color of the cursor
/// </param>
/// <param name="parent">
///     Panel, containing all figures
/// </param>
/// <param name="iniSize">
///     The initial size of the cursor
/// </param>
public class Cursor(Point2D<int> center, int radius, int speed, Brush color, Panel parent, Size iniSize) :
    Circle(center, radius, speed, color, parent, iniSize), ICursor
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
