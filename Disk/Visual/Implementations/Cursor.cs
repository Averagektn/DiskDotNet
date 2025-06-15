using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Disk.Data.Impl;
using Disk.Visual.Interfaces;

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
    private const int CentersCount = 3;
    private readonly Point2D<int>[] _centers = [new(), new(), new()];
    private int oldestCenter = 0;

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

    public virtual void MoveSmooth(Point2D<int> center)
    {
        const double alpha = 0.2;
        const double betta = 0.3;
        const double gamma = 0.5;
        double lerp = 0.3;

        _centers[oldestCenter] = center;
        oldestCenter++;
        oldestCenter %= CentersCount;
        /*        double x = (alpha * center.X) + ((1 - alpha) * Center.X);
                double y = (alpha * center.Y) + ((1 - alpha) * Center.Y);*/

        double avgX = (alpha * _centers[oldestCenter % CentersCount].X) +
            (betta * _centers[(oldestCenter + 1) % CentersCount].X) +
            (gamma * _centers[(oldestCenter + 2) % CentersCount].X);
        double avgY = (alpha * _centers[oldestCenter % CentersCount].Y) +
            (betta * _centers[(oldestCenter + 1) % CentersCount].Y) +
            (gamma * _centers[(oldestCenter + 2) % CentersCount].Y);
        double x = ((1 - lerp) * Center.X) + (lerp * avgX);
        double y = ((1 - lerp) * Center.Y) + (lerp * avgY);

        Move(new Point2D<int>((int)x, (int)y));
    }
}
