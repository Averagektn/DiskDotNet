using Disk.Data.Impl;

namespace Disk.Data.Interface
{
    /// <summary>
    ///     Represents a data source that provides coordinates of type <typeparamref name="CoordType"/>
    /// </summary>
    /// <typeparam name="CoordType">
    ///     The type of the coordinates
    /// </typeparam>
    interface IDataSource<CoordType>
        where CoordType :
            IConvertible,
            new()
    {
        /// <summary>
        ///     Gets the coordinates as a three-dimensional point in XYZ format
        /// </summary>
        /// <returns>
        ///     A <see cref="Point3D{CoordType}"/> object representing the coordinates in XYZ format, or null if the 
        ///     coordinates are not available
        /// </returns>
        Point3D<CoordType>? GetXYZ();

        /// <summary>
        ///     Gets the coordinates as a two-dimensional point in XY format
        /// </summary>
        /// <returns>
        ///     A <see cref="Point2D{CoordType}"/> object representing the coordinates in XY format, or null if the 
        ///     coordinates are not available
        /// </returns>
        Point2D<CoordType>? GetXY();

        /// <summary>
        ///     Gets the coordinates as a two-dimensional point in YZ format
        /// </summary>
        /// <returns>
        ///     A <see cref="Point2D{CoordType}"/> object representing the coordinates in YZ format, or null if the 
        ///     coordinates are not available
        /// </returns>
        Point2D<CoordType>? GetYZ();

        /// <summary>
        ///     Gets the coordinates as a two-dimensional point in XZ format
        /// </summary>
        /// <returns>
        ///     A <see cref="Point2D{CoordType}"/> object representing the coordinates in XZ format, or null if the 
        ///     coordinates are not available
        /// </returns>
        Point2D<CoordType>? GetXZ();

        /// <summary>
        ///     Gets the coordinates as a two-dimensional point in YX format
        /// </summary>
        /// <returns>
        ///     A <see cref="Point2D{CoordType}"/> object representing the coordinates in YX format, or null if the 
        ///     coordinates are not available
        /// </returns>
        Point2D<CoordType>? GetYX();

        /// <summary>
        ///     Gets the coordinates as a two-dimensional point in ZY format
        /// </summary>
        /// <returns>
        ///     A <see cref="Point2D{CoordType}"/> object representing the coordinates in ZY format, or null if the 
        ///     coordinates are not available
        /// </returns>
        Point2D<CoordType>? GetZY();

        /// <summary>
        ///     Gets the coordinates as a two-dimensional point in ZX format
        /// </summary>
        /// <returns>
        ///     A <see cref="Point2D{CoordType}"/> object representing the coordinates in ZX format, or null if the 
        ///     coordinates are not available
        /// </returns>
        Point2D<CoordType>? GetZX();
    }
}
