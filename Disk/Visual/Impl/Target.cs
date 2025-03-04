using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Disk.Visual.Impl;

/// <summary>
///     Represents a target with a center point, radius, and initial size
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
public class Target(Point2D<int> center, int radius, Canvas parent, Size iniSize) : 
    User(center, radius, 0, Brushes.White, parent, iniSize), ITarget
{
    /// <summary>
    ///     Invoked on <see cref="ReceiveShot(Point2D{int})"/> method call
    /// </summary>
    public event Action<int>? OnReceiveShot;

    /// <summary>
    ///     Gets the maximum radius of the target
    /// </summary>
    public virtual int MaxRadius => Radius * 5;

    /// <inheritdoc/>
    public override int Right => Center.X + MaxRadius;

    /// <inheritdoc/>
    public override int Top => Center.Y - MaxRadius;

    /// <inheritdoc/>
    public override int Bottom => Center.Y + MaxRadius;

    /// <inheritdoc/>
    public override int Left => Center.X - MaxRadius;

    /// <summary>
    ///     List of circles representing the concentric rings of the target
    /// </summary>
    protected readonly List<Circle> Circles =
        [
            new(center, radius * 5, 0, Brushes.Red, parent, iniSize),
            new(center, radius * 4, 0, Brushes.White, parent, iniSize),
            new(center, radius * 3, 0, Brushes.Red, parent, iniSize),
            new(center, radius * 2, 0, Brushes.White, parent, iniSize),
            new(center, radius * 1, 0, Brushes.Red, parent, iniSize)
        ];

    /// <inheritdoc/>
    public override void Draw()
    {
        Circles.ForEach(circle => circle.Draw());
        Scale();
        Parent.SizeChanged += Parent_SizeChanged;
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
        Circles.ForEach(circle => circle.Remove());
        Parent.SizeChanged -= Parent_SizeChanged;
    }
}
