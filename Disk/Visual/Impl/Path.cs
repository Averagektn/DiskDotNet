using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Disk.Visual.Impl;

/// <summary>
///     Represents a path that can be drawn and scaled
/// </summary>
public class Path : IStaticFigure
{
    /// <summary>
    ///     The polyline used to draw the path
    /// </summary>
    private readonly Polyline Polyline;

    /// <summary>
    ///     The list of points in the path
    /// </summary>
    private readonly List<Point2D<float>> Points = [];

    /// <summary>
    ///     The converter for coordinate transformations
    /// </summary>
    private readonly Converter Converter;

    /// <summary>
    ///     Required for correct positioning
    /// </summary>
    protected readonly Panel Parent;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Path"/> class
    /// </summary>
    /// <param name="points">
    ///     The points in the path
    /// </param>
    /// <param name="currSize">
    ///     The current size of the path
    /// </param>
    /// <param name="angleSize">
    ///     The size of the angle
    /// </param>
    /// <param name="color">
    ///     The color of the path
    /// </param>
    public Path(IEnumerable<Point2D<float>> points, Converter converter, Brush color, Panel parent)
    {
        Converter = converter;
        Parent = parent;

        Polyline = new Polyline()
        {
            Stroke = color,
            StrokeThickness = 2
        };

        foreach (var point in points)
        {
            Polyline.Points.Add(Converter.ToWndCoord(point).ToPoint());
            Points.Add(point);
        }
    }

    /// <inheritdoc/>
    public virtual void Draw()
    {
        _ = Parent.Children.Add(Polyline);
    }

    /// <inheritdoc/>
    public virtual void Remove()
    {
        Parent.Children.Remove(Polyline);
    }

    /// <inheritdoc/>
    public virtual void Scale()
    {
        Polyline.Points.Clear();
        Points.ForEach(point => Polyline.Points.Add(Converter.ToWndCoord(point).ToPoint()));
    }

    public bool Contains(Point2D<int> p)
    {
        return false;
    }
}
