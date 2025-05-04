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

/// <summary>
///     Represents a convex hull figure
/// </summary>
public class ConvexHull : IStaticFigure
{
    public static double GetArea<T>(List<Point2D<T>> points, float percent = 0.95f) where T : IConvertible, new()
    {
        var convexHullPoints = GetConvexHull(points, percent);

        int n = points.Count;
        double convexHullArea = 0;
        for (int i = 0; i < n; i++)
        {
            int j = (i + 1) % n;
            convexHullArea += convexHullPoints[i].X * convexHullPoints[j].Y;
            convexHullArea -= convexHullPoints[i].Y * convexHullPoints[j].X;
        }
        convexHullArea /= 2.0;

        return convexHullArea;
    }

    /// <summary>
    ///     Returns convex hull. OpenCV wrapper
    /// </summary>
    /// <typeparam name="T">Coord type</typeparam>
    /// <param name="points">List of points</param>
    /// <returns>Convex hull</returns>
    public static List<PointF> GetConvexHull<T>(List<Point2D<T>> points, float percent = 0.95f) where T : IConvertible, new()
    {
        var centerX = points.Count > 10 ? points.Average(p => p.XDbl) : 0.0;
        var centerY = points.Count > 10 ? points.Average(p => p.YDbl) : 0.0;
        var center = new Point2D<T>((T)Convert.ChangeType(centerX, typeof(T)), (T)Convert.ChangeType(centerY, typeof(T)));

        if (percent is > 1 or < 0)
        {
            percent = 0.95f;
        }

        var data = points
            .OrderBy(p => p.GetDistance(center))
            .Take((int)(points.Count * percent))
            .Select(p => p.ToPointF())
            .ToArray();

        return [.. CvInvoke.ConvexHull(data)];
    }

    /// <summary>
    ///    Shows if the figure is drawn 
    ///    Protects from multiple <see cref="Draw"/> calls
    /// </summary>
    public bool IsDrawn { get; private set; } = false;

    /// <summary>
    ///     Container for polygon
    /// </summary>
    protected readonly Panel Parent;

    /// <summary>
    ///     Scaling size
    /// </summary>
    protected readonly Size IniSize;

    private readonly Polygon _polygon;
    private readonly List<Point2D<int>> _points;

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
            Points = [.. GetConvexHull(points).Select(p => new Point(p.X, p.Y))]
        };
        _points = [.. points];
        Parent = parent;
        IniSize = iniSize;
    }

    /// <inheritdoc/>
    public virtual bool Contains(Point2D<int> p)
    {
        var path = new PathFigure
        {
            StartPoint = _polygon.Points[0],
            Segments = [new PolyLineSegment(_polygon.Points, true)]
        };

        var geometry = new PathGeometry([path]);

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
        if (!IsDrawn)
        {
            return;
        }

        Parent.Children.Remove(_polygon);
        Parent.SizeChanged -= Parent_SizeChanged;
        IsDrawn = false;
    }

    /// <inheritdoc/>
    public virtual void Scale()
    {
        double coeffX = Parent.ActualWidth / IniSize.Width;
        double coeffY = Parent.ActualHeight / IniSize.Height;

        var scaledPoints = _points.Select(p => new Point2D<int>((int)(p.X * coeffX), (int)(p.Y * coeffY))).ToList();
        var a = GetConvexHull(scaledPoints);

        _polygon.Points.Clear();
        a.ForEach(p => _polygon.Points.Add(new Point(p.X, p.Y)));
    }
}
