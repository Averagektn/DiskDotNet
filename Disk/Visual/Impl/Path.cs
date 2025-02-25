using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Size = System.Windows.Size;

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
    private readonly Panel _parent;
    public Path(IEnumerable<Point2D<float>> points, Converter converter, Brush color, Panel parent)
    {
        Converter = converter;
        _parent = parent;

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

    /// <summary>
    ///     Draws the path
    /// </summary>
    /// <param name="addChild">
    ///     The child element to add the path to
    /// </param>
    public void Draw()
    {
        _parent.Children.Add(Polyline);
    }

    /// <summary>
    ///     Removes the path from a UI element collection
    /// </summary>
    /// <param name="collection">
    ///     The UI element collection
    /// </param>
    public void Remove()
    {
        _parent.Children.Remove(Polyline);
    }

    /// <summary>
    ///     Scales the path to the specified size
    /// </summary>
    /// <param name="newSize">
    ///     The new size of the path
    /// </param>
    public void Scale()
    {
        Polyline.Points.Clear();

        foreach (var point in Points)
        {
            Polyline.Points.Add(Converter.ToWndCoord(point).ToPoint());
        }
    }
}
