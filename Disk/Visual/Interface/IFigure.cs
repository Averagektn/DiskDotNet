using Disk.Data.Impl;

namespace Disk.Visual.Interface
{
    /// <summary>
    ///     Represents a figure that can be drawn, scaled, and moved
    /// </summary>
    interface IFigure : IDrawable, IScalable, IMovable
    {
        Point2D<int> Center { get; }

        /// <summary>
        ///     Gets the right boundary of the figure
        /// </summary>
        int Right { get; }

        /// <summary>
        ///     Gets the top boundary of the figure
        /// </summary>
        int Top { get; }

        /// <summary>
        ///     Gets the bottom boundary of the figure
        /// </summary>
        int Bottom { get; }

        /// <summary>
        ///     Gets the left boundary of the figure
        /// </summary>
        int Left { get; }
    }
}
