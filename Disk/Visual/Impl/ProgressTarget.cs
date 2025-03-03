using Disk.Data.Impl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using XamlRadialProgressBar;

namespace Disk.Visual.Impl;

/// <summary>
///     Target with progress bar
/// </summary>
public class ProgressTarget : Target
{
    /// <summary>
    ///     Current <see cref="RadialProgressBar"/> value
    /// </summary>
    public double Progress => Border.Value / Border.Maximum;

    /// <summary>
    ///     Checks if <see cref="RadialProgressBar"/> is full
    /// </summary>
    public bool IsFull => (int)Border.Value >= (int)Border.Maximum;

    /// <inheritdoc/>
    public override Point2D<int> Center
    {
        get => base.Center;
        protected set
        {
            base.Center = value;

            Canvas.SetLeft(Border, Left);
            Canvas.SetTop(Border, Top);
        }
    }

    /// <inheritdoc/>
    public override int MaxRadius => Radius * 6;

    /// <summary>
    ///     Progress
    /// </summary>
    protected readonly RadialProgressBar Border;

    /// <summary>
    ///     Target with progress bar
    /// </summary>
    /// <param name="center">
    ///     The center point of the target
    /// </param>
    /// <param name="radius">
    ///     The radius of the target
    /// </param>
    /// <param name="parent">
    ///     Canvas, containing all figures
    /// </param>
    /// <param name="hp">
    ///     <see cref="RadialProgressBar"/> maximum value
    /// </param>
    /// <param name="iniSize">
    ///     The initial size of the target
    /// </param>
    public ProgressTarget(Point2D<int> center, int radius, Canvas parent, int hp, Size iniSize)
        : base(center, radius, parent, iniSize)
    {
        Border = new()
        {
            Maximum = hp,
            Foreground = Brushes.Blue,
            Width = radius * 6 * 2,
            Height = radius * 6 * 2,
        };
    }

    /// <summary>
    ///     Set <see cref="RadialProgressBar"/> value to 0
    /// </summary>
    public void Reset()
    {
        Border.Value = 0;
    }

    /// <inheritdoc/>
    public override int ReceiveShot(Point2D<int> shot)
    {
        var res = base.ReceiveShot(shot);
        Border.Value += res;

        return res;
    }

    /// <inheritdoc/>
    public override void Remove()
    {
        base.Remove();

        if (Parent.Children.Contains(Border))
        {
            Parent.Children.Remove(Border);
        }
    }

    /// <inheritdoc/>
    public override void Draw()
    {
        _ = Parent.Children.Add(Border);

        base.Draw();
    }

    /// <inheritdoc/>
    public override void Scale()
    {
        base.Scale();

        Border.Width = MaxRadius * 2;
        Border.Height = MaxRadius * 2;
    }
}
