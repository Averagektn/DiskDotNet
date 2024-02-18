using Disk.Data.Impl;

namespace Disk.Calculations.Impl.Converters
{
    /// <summary>
    ///     Provides conversion between coordinate systems
    /// </summary>
    internal partial class Converter
    {
        /// <summary>
        ///     Converts the X coordinate from angle space to window space
        /// </summary>
        /// <param name="angle">
        ///     The angle coordinate
        /// </param>
        /// <returns>
        ///     The converted X coordinate in window space
        /// </returns>
        public int ToWndCoordX(float angle)
        {
            angle += MaxAngle.Width;

            return (int)Math.Round(angle * ScreenSize.Width / AngleSize.Width);
        }

        /// <summary>
        ///     Converts the Y coordinate from angle space to window space
        /// </summary>
        /// <param name="angle">
        ///     The angle coordinate
        /// </param>
        /// <returns>
        ///     The converted Y coordinate in window space
        /// </returns>
        public int ToWndCoordY(float angle)
        {
            angle = -angle + MaxAngle.Height;

            return (int)Math.Round(angle * ScreenSize.Height / AngleSize.Height);
        }

        /// <summary>
        ///     Converts the X coordinate from logical space to window space
        /// </summary>
        /// <param name="logCoord">
        ///     The logical coordinate
        /// </param>
        /// <returns>
        ///     The converted X coordinate in window space
        /// </returns>
        public int ToWndCoordX(int logCoord) => (int)(logCoord + MaxLogCoord.Width);

        /// <summary>
        ///     Converts the Y coordinate from logical space to window space
        /// </summary>
        /// <param name="logCoord">
        ///     The logical coordinate
        /// </param>
        /// <returns>
        ///     The converted Y coordinate in window space
        /// </returns>
        public int ToWndCoordY(int logCoord) => (int)(MaxLogCoord.Height - logCoord);

        /// <summary>
        ///     Converts the coordinate from string representation to window space
        /// </summary>
        /// <param name="str">
        ///     The string representation of the coordinate
        /// </param>
        /// <param name="separator">
        ///     The separator character used in the string
        /// </param>
        /// <returns>
        ///     The converted coordinate in window space
        /// </returns>
        public static Point2D<int> ToWndCoord(string str, char separator)
        {
            var coords = str.Split(separator);

            return new Point2D<int>(int.Parse(coords[0]), int.Parse(coords[1]));
        }

        /// <summary>
        ///     Converts the 3D coordinate from string representation to window space
        /// </summary>
        /// <param name="str">
        ///     The string representation of the 3D coordinate
        /// </param>
        /// <param name="separator">
        ///     The separator character used in the string
        /// </param>
        /// <returns>
        ///     The converted 3D coordinate in window space
        /// </returns>
        public static Point3D<int> ToWndCoord3D(string str, char separator)
        {
            var coords = str.Split(separator);

            return new Point3D<int>(int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]));
        }

        /// <summary>
        ///     Converts the 2D angle coordinate to window space
        /// </summary>
        /// <param name="anglePoint">
        ///     The 2D angle coordinate
        /// </param>
        /// <returns>
        ///     The converted coordinate in window space
        /// </returns>
        public Point2D<int> ToWndCoord(Point2D<float> anglePoint)
            => new(ToWndCoordX(anglePoint.X), ToWndCoordY(anglePoint.Y));

        /// <summary>
        ///     Converts the 2D logical space to window space
        /// </summary>
        /// <param name="logPoint"> 
        ///     The 2D coordinate in logical space
        /// </param>
        /// <returns>
        ///     The converted coordinate in window space
        /// </returns>
        public Point2D<int> ToWndCoord(Point2D<int> logPoint) => new(ToWndCoordX(logPoint.X), ToWndCoordY(logPoint.Y));
    }
}
