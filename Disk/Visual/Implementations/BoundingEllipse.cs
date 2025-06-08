using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;

using Disk.Data.Impl;
using Disk.Visual.Interfaces;

using Emgu.CV;
using Emgu.CV.Util;

using Brush = System.Windows.Media.Brush;
using PathShape = System.Windows.Shapes.Path;
using Size = System.Windows.Size;

namespace Disk.Visual.Implementations;

/// <summary>
///     Represents a bounding ellipse with a center point, radius, and initial size
/// </summary>
public class BoundingEllipse : IStaticFigure
{
    /// <summary>
    ///   Calculates the area of the bounding ellipse for a set of points
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="points"></param>
    /// <param name="percent"></param>
    /// <returns></returns>
    public static double GetArea<T>(List<Point2D<T>> points, float percent = 0.95f) where T : IConvertible, new()
    {
        (PointF _, float radiusX, float radiusY, float _) = GetFillEllipse(points, percent);

        return Math.PI * radiusX * radiusY;
    }

    /// <summary>
    ///     Calculates the bounding ellipse for a set of points
    /// </summary>
    /// <typeparam name="T"> Coordinate type </typeparam>
    /// <param name="points"> Dataset </param>
    /// <param name="percent"> Cutoff percent </param>
    /// <returns> Bounding ellipse </returns>
    public static (PointF Center, float RadiusX, float RadiusY, float Angle) GetFillEllipse<T>(List<Point2D<T>> points,
        float percent = 0.95f) where T : IConvertible, new()
    {
        double centerX = points.Count > 10 ? points.Average(p => p.XDbl) : 0.0;
        double centerY = points.Count > 10 ? points.Average(p => p.YDbl) : 0.0;
        var center = new Point2D<T>((T)Convert.ChangeType(centerX, typeof(T)), (T)Convert.ChangeType(centerY, typeof(T)));

        if (percent is > 1 or < 0)
        {
            percent = 0.95f;
        }

        IEnumerable<PointF> dataset = points
            .OrderBy(p => p.GetDistance(center))
            .Take((int)(points.Count * percent))
            .Select(p => p.ToPointF());

        using var pointVector = new VectorOfPointF([.. dataset]);
        Emgu.CV.Structure.RotatedRect ellipse = CvInvoke.MinAreaRect(pointVector);

        return (ellipse.Center, ellipse.Size.Width / 2, ellipse.Size.Height / 2, ellipse.Angle);
    }

    /// <summary>
    ///    Shows if the figure is drawn 
    ///    Protects from multiple <see cref="Draw"/> calls
    /// </summary>
    public bool IsDrawn { get; private set; }

    /// <summary>
    ///    Ellipse rotation angle
    /// </summary>
    protected float RotationAngle { get; set; }

    /// <summary>
    ///    Drawing area
    /// </summary>
    protected Panel Parent;

    /// <summary>
    ///    Scaling size
    /// </summary>
    protected readonly Size IniSize;

    /// <summary>
    ///   Gets or sets the center of the ellipse
    /// </summary>
    protected virtual Point2D<int> Center
    {
        get => _center;
        set
        {
            _center = value;
            _ellipse.Center = _center.ToPoint();
        }
    }
    private Point2D<int> _center;

    /// <summary>
    ///   Gets or sets the X radius of the ellipse
    /// </summary>
    public virtual int RadiusX
    {
        get => _radiusX;
        protected set
        {
            _radiusX = value;
            _ellipse.RadiusX = RadiusX;
        }
    }
    private int _radiusX;

    /// <summary>
    ///    Gets or sets the Y radius of the ellipse
    /// </summary>
    public virtual int RadiusY
    {
        get => _radiusY;
        protected set
        {
            _radiusY = value;
            _ellipse.RadiusY = RadiusY;
        }
    }
    private int _radiusY;

    private readonly List<Point2D<int>> _points;
    private readonly PathShape _bound;
    private readonly EllipseGeometry _ellipse;
    private readonly Brush _brush;

    /// <summary>
    ///    Initializes a new instance of the <see cref="BoundingEllipse"/> class
    /// </summary>
    /// <param name="points">Dataset</param>
    /// <param name="boundColor">Bound color</param>
    /// <param name="parent">Drawing area</param>
    /// <param name="iniSize">Initial size for scaling</param>
    public BoundingEllipse(List<Point2D<int>> points, Brush boundColor, Panel parent, Size iniSize)
    {
        (PointF center, float radiusX, float radiusY, float rotationAngle) = GetFillEllipse(points);
        _points = [.. points];

        _center = new((int)center.X, (int)center.Y);
        _radiusX = (int)radiusX;
        _radiusY = (int)radiusY;
        RotationAngle = rotationAngle;

        _brush = boundColor;
        Parent = parent;
        IniSize = iniSize;

        _ellipse = new EllipseGeometry
        {
            Center = Center.ToPoint(),
            RadiusX = RadiusX,
            RadiusY = RadiusY,
        };
        UpdateTransform();
        _bound = new PathShape
        {
            Stroke = _brush,
            StrokeThickness = 2,
            Data = _ellipse
        };
    }

    /// <inheritdoc/>
    public virtual bool Contains(Point2D<int> p)
    {
        return _ellipse.FillContains(p.ToPoint());
    }

    /// <inheritdoc/>
    public virtual void Draw()
    {
        if (IsDrawn)
        {
            return;
        }

        _ = Parent.Children.Add(_bound);
        IsDrawn = true;
        Parent.SizeChanged += Parent_SizeChanged;
    }

    private void Parent_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
    {
        Scale();
    }

    private void UpdateTransform()
    {
        var rotateTransform = new RotateTransform(RotationAngle, Center.X, Center.Y);
        _ellipse.Transform = rotateTransform;
    }

    /// <inheritdoc/>
    public virtual void Remove()
    {
        if (!IsDrawn)
        {
            return;
        }

        Parent.Children.Remove(_bound);
        IsDrawn = false;
        Parent.SizeChanged -= Parent_SizeChanged;
    }

    /// <inheritdoc/>
    public virtual void Scale()
    {
        double coeffX = Parent.ActualWidth / IniSize.Width;
        double coeffY = Parent.ActualHeight / IniSize.Height;

        var scaledPoints = _points.Select(p => new Point2D<int>((int)(p.X * coeffX), (int)(p.Y * coeffY))).ToList();
        (PointF center, float radiusX, float radiusY, float rotationAngle) = GetFillEllipse(scaledPoints);

        RadiusX = (int)radiusX;
        RadiusY = (int)radiusY;
        RotationAngle = rotationAngle;
        Center = new Point2D<int>((int)center.X, (int)center.Y);
        UpdateTransform();
    }
}