using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Disk.Data.Impl;
using Disk.Visual.Interfaces;

using XamlRadialProgressBar;

namespace Disk.Visual.Implementations;

/// <summary>
///     Target with progress bar
/// </summary>
public class ProgressTarget : Target, IProgressTarget
{
    /// <inheritdoc/>
    public double Progress => Border.Value / Border.Maximum;

    /// <inheritdoc/>
    public bool IsFull => (int)Border.Value >= (int)Border.Maximum;

    /// <inheritdoc/>
    public override Point2D<int> Center
    {
        get => base.Center;
        protected set
        {
            base.Center = value;

            _borderTransform.X = Left;
            _borderTransform.Y = Top;
        }
    }
    private readonly TranslateTransform _borderTransform = new();

    /// <inheritdoc/>
    protected override int SingleRadius => (int)Math.Round((double)Radius / 6);

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
    ///     Panel, containing all figures
    /// </param>
    /// <param name="hp">
    ///     <see cref="RadialProgressBar"/> maximum value
    /// </param>
    /// <param name="iniSize">
    ///     The initial size of the target
    /// </param>
    public ProgressTarget(Point2D<int> center, int radius, Panel parent, int hp, Size iniSize)
        : base(center, radius, parent, iniSize)
    {
        Border = new()
        {
            Maximum = hp,
            Foreground = Brushes.Blue,
            Width = radius * 2,
            Height = radius * 2,
            RenderTransform = _borderTransform,
        };
    }

    /// <inheritdoc/>
    public void Reset()
    {
        Border.Value = 0;
    }

    /// <inheritdoc/>
    public override int ReceiveShot(Point2D<int> shot)
    {
        int res = base.ReceiveShot(shot);
        Border.Value += res;

        return res;
    }

    /// <inheritdoc/>
    public override void Remove()
    {
        if (!IsDrawn)
        {
            return;
        }

        base.Remove();

        if (Parent.Children.Contains(Border))
        {
            Parent.Children.Remove(Border);
        }
    }

    /// <inheritdoc/>
    public override void Draw()
    {
        if (IsDrawn)
        {
            return;
        }

        _ = Parent.Children.Add(Border);

        base.Draw();
    }

    /// <inheritdoc/>
    public override void Scale()
    {
        base.Scale();

        Border.Width = (Radius * 2) + 1;
        Border.Height = (Radius * 2) + 1;
    }
}
