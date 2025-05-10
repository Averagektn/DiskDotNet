using Disk.Data.Impl;
using Disk.Visual.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace Disk.Visual.Implementations;

/// <summary>
///     Same as <see cref="CursorPicture"/>, but can receive shots
/// </summary>
/// <remarks>
///     <inheritdoc/>
/// </remarks>
/// <param name="filePath">
///     Path to image
/// </param>
/// <param name="center">
///     The center point of the target
/// </param>
/// <param name="speed">
///     The speed of the circle
/// </param>
/// <param name="imageSize">
///     Initial size of the image
/// </param>
/// <param name="parent">
///     Panel, containing all figures
/// </param>
/// <param name="iniSize">
///     The initial size of the target
/// </param>
public class TargetPicture(string imageFilePath, Point2D<int> center, Size imageSize, Panel parent, Size iniSize, double hp)
    : CursorPicture(imageFilePath, center, 0, imageSize, parent, iniSize), IProgressTarget
{
    /// <summary>
    ///     Invoked on <see cref="ReceiveShot(Point2D{int})"/> method call
    /// </summary>
    public event Action<int>? OnReceiveShot;

    /// <inheritdoc/>
    public double Progress { get; protected set; }

    /// <inheritdoc/>
    public bool IsFull => Progress == Hp;

    /// <inheritdoc/>
    protected readonly double Hp = hp;

    /// <inheritdoc/>
    public virtual int ReceiveShot(Point2D<int> shot)
    {
        int res = Contains(shot) ? 1 : 0;

        Progress += res;

        OnReceiveShot?.Invoke(res);

        return res;
    }

    /// <inheritdoc/>
    public override bool Contains(Point2D<int> shot)
    {
        return Right >= shot.X && Left <= shot.X && Top <= shot.Y && Bottom >= shot.Y;
    }

    /// <summary>
    ///    Resets the progress of the target to 0
    /// </summary>
    public void Reset()
    {
        Progress = 0;
    }
}
