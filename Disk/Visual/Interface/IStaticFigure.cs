using Disk.Data.Impl;

namespace Disk.Visual.Interface;

/// <summary>
///     Figure that can't move
/// </summary>
public interface IStaticFigure : IDrawable, IScalable 
{
    /// <summary>
    ///     Determines whether the circle contains the specified point
    /// </summary>
    /// <param name="p">
    ///     The point to check
    /// </param>
    /// <returns>
    ///     true if the circle contains the point, otherwise false
    /// </returns>
    bool Contains(Point2D<int> p);
}
