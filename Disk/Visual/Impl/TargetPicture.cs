using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Controls;

namespace Disk.Visual.Impl;

/// <summary>
///     Same as <see cref="UserPicture"/>, but can receive shots
/// </summary>
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
///     Canvas, containing all figures
/// </param>
/// <param name="iniSize">
///     The initial size of the target
/// </param>
public class TargetPicture(string imageFilePath, Point2D<int> center, Size imageSize, Canvas parent, Size iniSize) :
    UserPicture(imageFilePath, center, 0, imageSize, parent, iniSize), ITarget
{
    /// <summary>
    ///     Invoked on <see cref="ReceiveShot(Point2D{int})"/> method call
    /// </summary>
    public event Action<int>? OnReceiveShot;

    /// <inheritdoc/>
    public virtual int ReceiveShot(Point2D<int> shot)
    {
        int res = Contains(shot) ? 1 : 0;

        OnReceiveShot?.Invoke(res);

        return res;
    }

    /// <inheritdoc/>
    public override bool Contains(Point2D<int> shot)
    {
        return Right <= shot.X && Left >= shot.X && Top >= shot.Y && Bottom <= shot.Y;
    }
}
