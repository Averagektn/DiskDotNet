using Disk.Data.Impl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using XamlRadialProgressBar;

namespace Disk.Visual.Impl;

public class ProgressTarget : Target
{
    public double Progress => _border.Value / _border.Maximum;

    public bool IsFull => (int)_border.Value >= (int)_border.Maximum;

    public override Point2D<int> Center
    {
        get => base.Center;
        protected set
        {
            base.Center = value;

            if (_border is not null)
            {
                Canvas.SetLeft(_border, Left);
                Canvas.SetTop(_border, Top);
            }
        }
    }

    /// <summary>
    ///     Gets the maximum radius of the target
    /// </summary>
    public override int MaxRadius => Radius * 6;

    /// <summary>
    ///     Gets the X-coordinate of the right edge of the circle
    /// </summary>
    public override int Right => Center.X + MaxRadius;

    /// <summary>
    ///     Gets the Y-coordinate of the top edge of the circle
    /// </summary>
    public override int Top => Center.Y - MaxRadius;

    /// <summary>
    ///     Gets the Y-coordinate of the bottom edge of the circle
    /// </summary>
    public override int Bottom => Center.Y + MaxRadius;

    /// <summary>
    ///     Gets the X-coordinate of the left edge of the circle
    /// </summary>
    public override int Left => Center.X - MaxRadius;

    private readonly RadialProgressBar _border;

    public ProgressTarget(Point2D<int> center, int radius, Canvas parent, int hp, Size iniSize)
        : base(center, radius, parent, iniSize)
    {
        _border = new()
        {
            Maximum = hp,
            Foreground = Brushes.Blue,
            Width = radius * 6 * 2,
            Height = radius * 6 * 2,
        };
    }

    public void Reset()
    {
        _border.Value = 0;
    }

    public override int ReceiveShot(Point2D<int> shot)
    {
        var res = base.ReceiveShot(shot);
        _border.Value += res;

        return res;
    }

    public override void Remove()
    {
        base.Remove();

        if (Parent.Children.Contains(_border))
        {
            Parent.Children.Remove(_border);
        }
    }

    /// <summary>
    ///     Draws the target by adding each circle to the specified UI element collection
    /// </summary>
    /// <param name="addChild">
    ///     The UI element collection to add the circles to
    /// </param>
    public override void Draw()
    {
        _ = Parent.Children.Add(_border);
        base.Draw();
    }

    public override void Scale()
    {
        base.Scale();

        _border.Width = MaxRadius * 2;
        _border.Height = MaxRadius * 2;
    }
}
