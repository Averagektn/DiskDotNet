using Disk.Data.Impl;

namespace Disk.Visual.Interface;

/// <summary>
///     Shot receivable object
/// </summary>
public interface ITarget : IDynamicFigure, IRoundedFigure
{
    /// <summary>
    ///     Actions when shot is received
    /// </summary>
    /// <param name="shot">
    ///     Point of shot
    /// </param>
    /// <returns>
    ///     Can be represented by number of points, received after shot. 0 should be interpreted as a miss
    /// </returns>
    int ReceiveShot(Point2D<int> shot);

    /// <summary>
    ///     Invoked on <see cref="ReceiveShot(Point2D{int})"/> method call
    /// </summary>
    event Action<int>? OnReceiveShot;
}
