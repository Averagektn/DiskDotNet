using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Size = System.Windows.Size;

namespace Disk.Visual.Impl;

/// <summary>
///     Represents a Circle figure that implements the IFigure interface
/// </summary>
public class Circle : IDynamicFigure
{
    private Point2D<int> _center = new();
    /// <summary>
    ///     <inheritdoc/>
    ///     Center change will force positioning change with
    ///     <see cref="Canvas.SetLeft(System.Windows.UIElement, double)"/> and
    ///     <see cref="Canvas.SetTop(System.Windows.UIElement, double)"/> of all figures
    /// </summary>
    public virtual Point2D<int> Center
    {
        get => _center;
        protected set
        {
            _center = value;

            Canvas.SetLeft(Figure, Left);
            Canvas.SetTop(Figure, Top);
        }
    }

    /// <summary>
    ///     <inheritdoc/>
    /// </summary>
    public virtual int Right => Center.X + Radius;

    /// <summary>
    ///     <inheritdoc/>
    /// </summary>
    public virtual int Top => Center.Y - Radius;

    /// <summary>
    ///     <inheritdoc/>
    /// </summary>
    public virtual int Bottom => Center.Y + Radius;

    /// <summary>
    ///     <inheritdoc/>
    /// </summary>
    public virtual int Left => Center.X - Radius;

    private int _radius = 0;
    /// <summary>
    ///     Gets or sets the radius of the circle. 
    ///     Change will force resize
    /// </summary>
    public virtual int Radius
    {
        get => _radius;
        protected set
        {
            _radius = value;
            Figure.Height = value * 2;
            Figure.Width = value * 2;
        }
    }

    /// <summary>
    ///     Gets or sets the speed of the circle
    /// </summary>
    public int Speed { get; protected set; }

    /// <summary>
    ///     Correction multiplier for diagonal movement
    /// </summary>
    protected const float DiagonalCorrection = 1.41f;

    /// <summary>
    ///     Figure to be drawn
    /// </summary>
    private readonly Ellipse Figure;

    /// <summary>
    ///     Initial size for scaling
    /// </summary>
    protected readonly Size IniSize;

    /// <summary>
    ///     Size before parent size is changed
    /// </summary>
    protected Size CurrSize;

    /// <summary>
    ///     Initial speed to be scaled
    /// </summary>
    private readonly int IniSpeed;

    /// <summary>
    ///     Initial radius to be scaled
    /// </summary>
    private readonly int IniRadius;

    /// <summary>
    ///     Required for correct positioning
    /// </summary>
    protected readonly Canvas Parent;

    /// <summary>
    ///     Initializes a new instance of the Circle class with the specified center, radius, speed, color 
    ///     and initial size
    /// </summary>
    /// <param name="center">
    ///     The center point of the circle
    /// </param>
    /// <param name="radius">
    ///     The radius of the circle
    /// </param>
    /// <param name="speed">
    ///     The speed of the circle
    /// </param>
    /// <param name="color">
    ///     The color of the circle
    /// </param>
    /// <param name="iniSize">
    ///     The initial size of the circle
    /// </param>
    public Circle(Point2D<int> center, int radius, int speed, Brush color, Canvas canvas, Size iniSize)
    {
        IniRadius = radius;
        IniSpeed = speed;

        Parent = canvas;

        Figure = new()
        {
            Width = radius * 2,
            Height = radius * 2,
            Fill = color
        };

        Speed = speed;
        _radius = radius;
        _center = center;

        IniSize = iniSize;
        CurrSize = iniSize;
    }

    /// <inheritdoc/>
    public virtual bool Contains(Point2D<int> p)
    {
        return Math.Sqrt(Math.Pow((p.X - Center.X) / Radius, 2) + Math.Pow((p.Y - Center.Y) / Radius, 2)) <= 0;
    }

    /// <inheritdoc/>
    public virtual void Draw()
    {
        Scale();
        _ = Parent.Children.Add(Figure);
        Parent.SizeChanged += Parent_SizeChanged;
    }

    /// <summary>
    ///     Used for subscription and unsubscription on <see cref="Parent"/> SizeChanged event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void Parent_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
    {
        Scale();
    }

    /// <inheritdoc/>
    public virtual void Remove()
    {
        Parent.Children.Remove(Figure);
        Parent.SizeChanged -= Parent_SizeChanged;
    }

    /// <inheritdoc/>
    public virtual void Move(bool moveTop, bool moveRight, bool moveBottom, bool moveLeft)
    {
        int xSpeed = 0;
        int ySpeed = 0;
        int speed = Speed;

        if ((moveTop || moveBottom) && (moveRight || moveLeft))
        {
            speed = (int)Math.Round(speed / DiagonalCorrection);
        }

        if (moveTop)
        {
            ySpeed -= speed;
        }
        if (moveBottom)
        {
            ySpeed += speed;
        }
        if (moveLeft)
        {
            xSpeed -= speed;
        }
        if (moveRight)
        {
            xSpeed += speed;
        }

        if (Left <= 0 && xSpeed < 0)
        {
            xSpeed = 0;
        }
        if (Right >= Parent.ActualWidth && xSpeed > 0)
        {
            xSpeed = 0;
        }
        if (Top <= 0 && ySpeed < 0)
        {
            ySpeed = 0;
        }
        if (Bottom >= Parent.ActualHeight && ySpeed > 0)
        {
            ySpeed = 0;
        }

        Center = new(Center.X + xSpeed, Center.Y + ySpeed);
    }

    /// <inheritdoc/>
    public virtual void Scale()
    {
        double coeffX = Parent.ActualWidth / IniSize.Width;
        double coeffY = Parent.ActualHeight / IniSize.Height;

        //Speed = (int)Math.Round(IniSpeed * (coeffX + coeffY) / 2);
        //Radius = (int)Math.Round(IniRadius * (coeffX + coeffY) / 2);
        
        Speed = (int)Math.Round(IniSpeed * Math.Min(coeffX, coeffY));
        Radius = (int)Math.Round(IniRadius * Math.Min(coeffX, coeffY));

        Center = new
        (
            (int)Math.Round(Center.X * (Parent.ActualWidth / CurrSize.Width)),
            (int)Math.Round(Center.Y * (Parent.ActualHeight / CurrSize.Height))
        );

        CurrSize = Parent.RenderSize;
    }

    /// <inheritdoc/>
    public virtual void Move(Point2D<int> center)
    {
        if (center.X <= Parent.ActualWidth && center.Y <= Parent.ActualHeight && center.X > 0 && center.Y > 0)
        {
            Center = center;
        }
    }
}
