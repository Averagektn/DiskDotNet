using Disk.Data.Impl;

namespace Disk.Calculations.Impl.Converters
{
    /// <summary>
    ///     Provides conversion between coordinate systems
    /// </summary>
    internal partial class Converter
    {
        /// <summary>
        ///     Converts the X coordinate from window space to logical space
        /// </summary>
        /// <param name="coord">
        ///     The X coordinate in window space
        /// </param>
        /// <returns>
        ///     The converted X coordinate in logical space
        /// </returns>
        public int ToLogCoordX(int coord) => (int)(coord - MaxLogCoord.Width);

        /// <summary>
        ///     Converts the Y coordinate from window space to logical space
        /// </summary>
        /// <param name="coord">
        ///     The Y coordinate in window space
        /// </param>
        /// <returns>
        ///     The converted Y coordinate in logical space
        /// </returns>
        public int ToLogCoordY(int coord) => (int)(MaxLogCoord.Height - coord);

        /// <summary>
        ///     Converts the X coordinate from angle space to logical space
        /// </summary>
        /// <param name="angle">
        ///     The angle coordinate
        /// </param>
        /// <returns>
        ///     The converted X coordinate in logical space
        /// </returns>
        public int ToLogCoordX(float angle) => ToLogCoordX(ToWndCoordX(angle));

        /// <summary>
        ///     Converts the Y coordinate from angle space to logical space
        /// </summary>
        /// <param name="angle">
        ///     The angle coordinate
        /// </param>
        /// <returns>
        ///     The converted Y coordinate in logical space
        /// </returns>
        public int ToLogCoordY(float angle) => ToLogCoordY(ToWndCoordY(angle));

        /// <summary>
        ///     Converts the 2D angle coordinate to logical space
        /// </summary>
        /// <param name="point">
        ///     The 2D angle coordinate
        /// </param>
        /// <returns>
        ///     The converted coordinate in logical space
        /// </returns>
        public Point2D<int> ToLogCoord(Point2D<float> point) => new(ToLogCoordX(point.X), ToLogCoordY(point.Y));

        /// <summary>
        ///     Converts the 2D logarithmic coordinate to logical space.
        /// </summary>
        /// <param name="point">
        ///     The 2D logarithmic coordinate
        /// </param>
        /// <returns>
        ///     The converted coordinate in logical space
        /// </returns>
        public Point2D<int> ToLogCoord(Point2D<int> point) => new(ToLogCoordX(point.X), ToLogCoordY(point.Y));

        /// <summary>
        ///     Converts the coordinate from string representation to logical space
        /// </summary>
        /// <param name="str">
        ///     The string representation of the coordinate
        /// </param>
        /// <param name="separator">
        ///     The separator character used in the string
        /// </param>
        /// <returns>
        ///     The converted coordinate in logical space
        /// </returns>
        public static Point2D<int> ToLogCoord(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(int.Parse(coords[0]), int.Parse(coords[1]));
        }

        /// <summary>
        ///     Converts the 3D coordinate from string representation to logical space
        /// </summary>
        /// <param name="str">
        ///     The string representation of the 3D coordinate
        /// </param>
        /// <param name="separator">
        ///     The separator character used in the string
        /// </param>
        /// <returns>
        ///     The converted 3D coordinate in logical space
        /// </returns>
        public static Point3D<int> ToLogCoord3D(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]));
        }
    }
}
