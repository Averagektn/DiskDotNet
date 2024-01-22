using Disk.Data.Impl;

namespace Disk.Data.Interface
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="CoordType">
    /// 
    /// </typeparam>
    interface IDataSource<CoordType>
        where CoordType :
            IConvertible,
            new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        Point3D<CoordType>? GetXYZ();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        Point2D<CoordType>? GetXY();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        Point2D<CoordType>? GetYZ();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        Point2D<CoordType>? GetXZ();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        Point2D<CoordType>? GetYX();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        Point2D<CoordType>? GetZY();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        Point2D<CoordType>? GetZX();
    }
}
