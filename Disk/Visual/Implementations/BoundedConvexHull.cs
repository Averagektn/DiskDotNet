using Disk.Data.Impl;
using Disk.Visual.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Disk.Visual.Implementations;

/// <summary>
///     Represents a bounded convex hull figure
/// </summary>
/// <param name="points">Dataset</param>
/// <param name="borderColor">Border</param>
/// <param name="fillColor">Fill</param>
/// <param name="parent">Drawing area</param>
/// <param name="iniSize">Scaling size</param>
public class BoundedConvexHull(List<Point2D<int>> points, Brush borderColor, Brush fillColor, Panel parent, Size iniSize)
    : IStaticFigure
{
    /// <summary>
    ///   Inner figure
    /// </summary>
    protected ConvexHull ConvexHull = new(points, borderColor, fillColor, parent, iniSize);

    /// <summary>
    ///   Outer bounding figure
    /// </summary>
    protected BoundingEllipse Bound = new(points, borderColor, parent, iniSize);

    /// <inheritdoc />
    public bool Contains(Point2D<int> p)
    {
        return Bound.Contains(p) || ConvexHull.Contains(p);
    }

    /// <inheritdoc />
    public void Draw()
    {
        ConvexHull.Draw();
        Bound.Draw();
    }

    /// <inheritdoc />
    public void Remove()
    {
        Bound.Remove();
        ConvexHull.Remove();
    }

    /// <inheritdoc />
    public void Scale()
    {
        ConvexHull.Scale();
        Bound.Scale();
    }
}
