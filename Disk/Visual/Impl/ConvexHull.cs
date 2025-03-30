using Disk.Data.Impl;
using Disk.Visual.Interface;
using Emgu.CV;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Brush = System.Windows.Media.Brush;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Disk.Visual.Impl;

public class ConvexHull : IStaticFigure
{
    /// <summary>
    ///     Protects from multiple <see cref="Draw"/> calls
    /// </summary>
    public bool IsDrawn { get; private set; }

    /// <summary>
    ///     Container for polygon
    /// </summary>
    protected readonly Panel Parent;

    /// <summary>
    ///     Scaling size
    /// </summary>
    protected Size CurrSize;

    private readonly Polygon _polygon;

    /// <summary>
    ///     Crates a covex hull based on points list
    /// </summary>
    /// <param name="points">Points inside the hull</param>
    /// <param name="borderColor">Stroke</param>
    /// <param name="fillColor">Fill</param>
    /// <param name="parent">Parent</param>
    /// <param name="iniSize">Start size</param>
    public ConvexHull(List<Point2D<int>> points, Brush borderColor, Brush fillColor, Panel parent, Size iniSize)
    {
        _polygon = new Polygon()
        {
            Stroke = borderColor,
            StrokeThickness = 2,
            Fill = fillColor,
            Points = new([.. points.Select(p => p.ToPoint())])
        };
        Parent = parent;
        CurrSize = iniSize;
    }

    /// <summary>
    ///     Returns convex hull. OpenCV wrapper
    /// </summary>
    /// <typeparam name="T">Coord type</typeparam>
    /// <param name="points">List of points</param>
    /// <returns>Convex hull</returns>
    public static List<PointF> GetConvexHull<T>(List<Point2D<T>> points) where T : IConvertible, new()
    {
        var data = points.Select(p => p.ToPointF()).ToArray();

        return [.. CvInvoke.ConvexHull(data)];
    }

    /// <inheritdoc/>
    public virtual bool Contains(Point2D<int> p)
    {
        var geometry = new PathGeometry(
            [
                new PathFigure
                {
                    StartPoint = _polygon.Points[0],
                    Segments = [new PolyLineSegment(_polygon.Points, true)]
                }
            ]);

        return geometry.FillContains(p.ToPoint());
    }

    /// <inheritdoc/>
    public virtual void Draw()
    {
        if (IsDrawn)
        {
            return;
        }

        _ = Parent.Children.Add(_polygon);
        Parent.SizeChanged += Parent_SizeChanged;
        IsDrawn = true;
    }

    private void Parent_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
    {
        Scale();
    }

    /// <inheritdoc/>
    public virtual void Remove()
    {
        Parent.Children.Remove(_polygon);
        Parent.SizeChanged -= Parent_SizeChanged;
        IsDrawn = false;
    }

    /// <inheritdoc/>
    public virtual void Scale()
    {
        double xScale = Parent.ActualWidth / CurrSize.Width;
        double yScale = Parent.ActualHeight / CurrSize.Height;

        var points = new List<Point>(_polygon.Points.Count);
        foreach (var item in _polygon.Points)
        {
            int x = (int)(item.X * xScale);
            int y = (int)(item.Y * yScale);
            points.Add(new Point(x, y));
        }
        _polygon.Points = [.. points];
        CurrSize = Parent.RenderSize;
    }
}
