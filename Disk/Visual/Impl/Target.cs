using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Disk.Visual.Impl;

/// <summary>
///     Represents a target with a center point, radius, and initial size
/// </summary>
public class Target : User, ITarget
{
    /// <inheritdoc/>
    public event Action<int>? OnReceiveShot;

    /// <inheritdoc/>
    public override int Right => Center.X + Radius;

    /// <inheritdoc/>
    public override int Top => Center.Y - Radius;

    /// <inheritdoc/>
    public override int Bottom => Center.Y + Radius;

    /// <inheritdoc/>
    public override int Left => Center.X - Radius;

    /// <summary>
    ///     Gets the radius of smallest target
    /// </summary>
    protected virtual int SingleRadius => (int)Math.Round((double)Radius / 5);

    /// <summary>
    ///     List of circles representing the concentric rings of the target
    /// </summary>
    protected List<Circle> Circles;

    /// <summary>
    ///     <inheritdoc/>
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
    /// <param name="iniSize">
    ///     The initial size of the target
    /// </param>
    public Target(Point2D<int> center, int radius, Canvas parent, Size iniSize) :
        base(center, radius, 0, Brushes.Transparent, parent, iniSize)
    {
        Circles = [
            new(center, SingleRadius * 5, 0, Brushes.Red, parent, iniSize),
            new(center, SingleRadius * 4, 0, Brushes.White, parent, iniSize),
            new(center, SingleRadius * 3, 0, Brushes.Red, parent, iniSize),
            new(center, SingleRadius * 2, 0, Brushes.White, parent, iniSize),
            new(center, SingleRadius * 1, 0, Brushes.Red, parent, iniSize)
        ];
    }

    /// <inheritdoc/>
    public override void Draw()
    {
        if (IsDrawn)
        {
            return;
        }

        Circles.ForEach(circle => circle.Draw());
        Scale();
        Parent.SizeChanged += Parent_SizeChanged;
        base.Draw();
    }

    /// <inheritdoc/>
    public override void Move(bool moveTop, bool moveRight, bool moveBottom, bool moveLeft)
    {
        base.Move(moveTop, moveRight, moveBottom, moveLeft);

        Circles.ForEach(circle => circle.Move(moveTop, moveRight, moveBottom, moveLeft));
    }

    /// <inheritdoc/>
    public override void Move(Point2D<int> center)
    {
        base.Move(center);
        Circles.ForEach(circle => circle.Move(Center));
    }

    /// <inheritdoc/>
    public virtual int ReceiveShot(Point2D<int> shot)
    {
        int res = Circles.Any(circle => circle.Contains(shot)) ? 1 : 0;
        // uncomment to make progressive target fill
        // int res = Circles.Count(circle => circle.Contains(shot));

        OnReceiveShot?.Invoke(res);

        return res;
    }

    /// <inheritdoc/>
    public override bool Contains(Point2D<int> shot)
    {
        bool contains = false;

        foreach (var circle in Circles)
        {
            contains |= circle.Contains(shot);
        }

        return contains;
    }

    /// <inheritdoc/>
    public override void Remove()
    {
        if (!IsDrawn)
        {
            return;
        }

        Circles.ForEach(circle => circle.Remove());
        Parent.SizeChanged -= Parent_SizeChanged;
        base.Remove();
    }
}
