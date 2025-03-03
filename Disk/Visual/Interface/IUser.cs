using Disk.Data.Impl;

namespace Disk.Visual.Interface;

/// <summary>
///     User or cursor
/// </summary>
public interface IUser
{
    /// <summary>
    ///     After shot actions
    /// </summary>
    event Action<Point2D<int>>? OnShot;

    /// <summary>
    ///     Clear <see cref="OnShot"/> event
    /// </summary>
    void ClearOnShot();

    /// <summary>
    ///     Invokes <see cref="OnShot"/>
    /// </summary>
    /// <returns>
    ///     Shot position
    /// </returns>
    Point2D<int> Shot();
}
