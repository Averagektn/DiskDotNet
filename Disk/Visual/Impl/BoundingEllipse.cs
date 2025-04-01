using Disk.Data.Impl;
using Disk.Visual.Interface;
using Emgu.CV;
using Emgu.CV.Util;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Size = System.Windows.Size;

namespace Disk.Visual.Impl;

public class BoundingEllipse : IStaticFigure
{
    public static (PointF Center, float RadiusX, float RadiusY, float Angle) GetFillEllipse<T>(List<Point2D<T>> points,
        float percent = 0.95f) where T : IConvertible, new()
    {
        var dataset = points.ToList();

        if (points.Count < 100 || percent > 1 || percent < 0)
        {
            percent = 1;
        }

        var ch = ConvexHull.GetConvexHull(dataset, percent);
        if (ch.Count < 5)
        {
            ch = [.. dataset.Select(p => p.ToPointF())];
        }
        using var pointVector = new VectorOfPointF([.. ch]);
        var ellipse = CvInvoke.FitEllipse(pointVector);
        if (ellipse.Angle == 0)
        {
            using var newVector = new VectorOfPointF([.. dataset.Select(p => p.ToPointF())]);
            ellipse = CvInvoke.FitEllipse(newVector);
        }

        return (ellipse.Center, ellipse.Size.Width / 2, ellipse.Size.Height / 2, ellipse.Angle);
    }

    protected Panel Parent;
    protected readonly Size IniSize;

    private readonly System.Windows.Shapes.Path _bound;
    private readonly EllipseGeometry _ellipse;
    private readonly Brush _brush;

    public bool IsDrawn { get; private set; } = false;
    protected float RotationAngle { get; set; }

    private Point2D<int> _center;
    protected virtual Point2D<int> Center
    {
        get => _center;
        set
        {
            _center = value;
            _ellipse.Center = _center.ToPoint();
        }
    }

    private int _radiusX;
    public virtual int RadiusX
    {
        get => _radiusX;
        protected set
        {
            _radiusX = value;
            _ellipse.RadiusX = RadiusX;
        }
    }

    private int _radiusY;
    public virtual int RadiusY
    {
        get => _radiusY;
        protected set
        {
            _radiusY = value;
            _ellipse.RadiusY = RadiusY;
        }
    }

    public virtual int Right => Center.X + RadiusX;
    public virtual int Top => Center.Y - RadiusY;
    public virtual int Bottom => Center.Y + RadiusY;
    public virtual int Left => Center.X - RadiusX;

    private readonly List<Point2D<int>> _points;

    public BoundingEllipse(List<Point2D<int>> points, Brush boundColor, Panel parent, Size iniSize)
    {
        var (center, radiusX, radiusY, rotationAngle) = GetFillEllipse(points);
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
        _bound = new System.Windows.Shapes.Path
        {
            Stroke = _brush,
            StrokeThickness = 2,
            Data = _ellipse
        };
    }

    private void UpdateTransform()
    {
        var rotateTransform = new RotateTransform(RotationAngle, Center.X, Center.Y);
        _ellipse.Transform = rotateTransform;
    }

    public virtual bool Contains(Point2D<int> p)
    {
        return _ellipse.FillContains(p.ToPoint());
    }

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

    public virtual void Remove()
    {
        Parent.Children.Remove(_bound);
        IsDrawn = false;
        Parent.SizeChanged -= Parent_SizeChanged;
    }

    public virtual void Scale()
    {
        double coeffX = Parent.ActualWidth / IniSize.Width;
        double coeffY = Parent.ActualHeight / IniSize.Height;

        var (center, radiusX, radiusY, rotationAngle) =
            GetFillEllipse(_points.Select(p => new Point2D<int>((int)(p.X * coeffX), (int)(p.Y * coeffY))).ToList());
        RadiusX = (int)radiusX;
        RadiusY = (int)radiusY;
        RotationAngle = rotationAngle;
        Center = new((int)center.X, (int)center.Y);
        UpdateTransform();
    }
}