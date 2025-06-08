using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using Disk.Data.Impl;
using Disk.Visual.Interfaces;

namespace Disk.Visual.Implementations;

/// <summary>
///     Represents a path that can be drawn and scaled
/// </summary>
public class Path : IStaticFigure
{
    /// <summary>
    ///    Shows if the figure is drawn 
    ///    Protects from multiple <see cref="Draw"/> calls
    /// </summary>
    public bool IsDrawn { get; private set; }

    /// <summary>
    ///     Required for correct positioning
    /// </summary>
    protected readonly Panel Parent;

    /// <summary>
    ///    The size of the drawing area
    /// </summary>
    protected readonly Size IniSize;

    /// <summary>
    ///     The polyline used to draw the path
    /// </summary>
    private readonly Polyline _polyline;

    private readonly List<Point2D<int>> _iniPoints;

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
    public Path(IEnumerable<Point2D<int>> points, Brush color, Panel parent, Size iniSize)
    {
        _iniPoints = [.. points];
        Parent = parent;

        _polyline = new Polyline()
        {
            Stroke = color,
            StrokeThickness = 2
        };

        _iniPoints.ForEach(point => _polyline.Points.Add(point.ToPoint()));

        IniSize = iniSize;
    }

    /// <inheritdoc/>
    public virtual void Draw()
    {
        if (IsDrawn)
        {
            return;
        }

        _ = Parent.Children.Add(_polyline);
        Parent.SizeChanged += Parent_SizeChanged;
        IsDrawn = true;
    }

    private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        Scale();
    }

    /// <inheritdoc/>
    public virtual void Remove()
    {
        if (!IsDrawn)
        {
            return;
        }

        Parent.Children.Remove(_polyline);
        Parent.SizeChanged -= Parent_SizeChanged;
        IsDrawn = false;
    }

    /// <inheritdoc/>
    public virtual void Scale()
    {
        double xScale = Parent.ActualWidth / IniSize.Width;
        double yScale = Parent.ActualHeight / IniSize.Height;

        _polyline.Points.Clear();
        _iniPoints.ForEach(point => _polyline.Points.Add(new Point(point.X * xScale, point.Y * yScale)));
    }

    /// <inheritdoc/>
    public bool Contains(Point2D<int> p)
    {
        return false;
    }
}
