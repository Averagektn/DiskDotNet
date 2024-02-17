using Disk.Data.Impl;

namespace Disk.Calculations.Impl.Converters
{
    /// <summary>
    ///     Provides conversion between coordinate systems
    /// </summary>
    internal partial class Converter
    {
        /// <summary>
        ///     Converts the X coordinate from window space to angle space
        /// </summary>
        /// <param name="wndCoord">
        ///     The X coordinate in window space
        /// </param>
        /// <returns>
        ///     The converted X coordinate in angle space
        /// </returns>
        public float ToAngleX_FromWnd(int wndCoord) => (float)(ToLogCoordX(wndCoord) * AngleSize.Width / ScreenSize.Width);

        /// <summary>
        ///     Converts the Y coordinate from window space to angle space
        /// </summary>
        /// <param name="wndCoord">
        ///     The Y coordinate in window space
        /// </param>
        /// <returns>
        ///     The converted Y coordinate in angle space
        /// </returns>
        public float ToAngleY_FromWnd(int wndCoord) => (float)(ToLogCoordY(wndCoord) * AngleSize.Height / ScreenSize.Height);

        /// <summary>
        ///     Converts the X coordinate from logical space to angle space
        /// </summary>
        /// <param name="logCoord">
        ///     The X coordinate in logical space
        /// </param>
        /// <returns>
        ///     The converted X coordinate in angle space
        /// </returns>
        public float ToAngleX_FromLog(int logCoord) => ToAngleX_FromWnd(ToWndCoordX(logCoord));

        /// <summary>
        ///     Converts the Y coordinate from logical space to angle space
        /// </summary>
        /// <param name="logCoord">
        ///     The Y coordinate in logical space
        /// </param>
        /// <returns>
        ///     The converted Y coordinate in angle space
        /// </returns>
        public float ToAngleY_FromLog(int logCoord) => ToAngleY_FromWnd(ToWndCoordY(logCoord));

        /// <summary>
        ///     Converts the coordinate from string representation to angle space
        /// </summary>
        /// <param name="str">
        ///     The string representation of the coordinate
        /// </param>
        /// <param name="separator">
        ///     The separator character used in the string
        /// </param>
        /// <returns>
        ///     The converted coordinate in angle space
        /// </returns>
        public static Point2D<float> ToAngle(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(float.Parse(coords[0]), float.Parse(coords[1]));
        }

        /// <summary>
        ///     Converts the 3D coordinate from string representation to angle space
        /// </summary>
        /// <param name="str">
        ///     The string representation of the 3D coordinate
        /// </param>
        /// <param name="separator">
        ///     The separator character used in the string
        /// </param>
        /// <returns>
        ///     The converted 3D coordinate in angle space
        /// </returns>
        public static Point3D<float> ToAngle3D(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));
        }

        /// <summary>
        ///     Converts the 2D coordinate from window space to angle space
        /// </summary>
        /// <param name="point">
        ///     The 2D coordinate in window space
        /// </param>
        /// <returns>
        ///     The converted coordinate in angle space
        /// </returns>
        public Point2D<float> ToAngle_FromWnd(Point2D<int> point)
            => new(ToAngleX_FromWnd(point.X), ToAngleY_FromWnd(point.Y));

        /// <summary>
        ///     Converts the 2D coordinate from logical space to angle space
        /// </summary>
        /// <param name="logPoint">
        ///     The 2D coordinate in logical space
        /// </param>
        /// <returns>
        ///     The converted coordinate in angle space
        /// </returns>
        public Point2D<float> ToAngle_FromLog(Point2D<int> logPoint)
            => new(ToAngleX_FromLog(logPoint.X), ToAngleY_FromLog(logPoint.Y));

        /// <summary>
        ///     Converts the angle from radian to degrees
        /// </summary>
        /// <param name="radian">
        ///     The angle in radians
        /// </param>
        /// <returns>
        ///     The converted angle in degrees
        /// </returns>
        public static float ToAngle_FromRadian(float radian) => (float)(radian * 180 / Math.PI);

        /// <summary>
        ///     Converts the 2D angle coordinate from radian to degrees
        /// </summary>
        /// <param name="radian">
        ///     The 2D angle coordinate in radians
        /// </param>
        /// <returns>
        ///     The converted 2D angle coordinate in degrees
        /// </returns>
        public static Point2D<float> ToAngle_FromRadian(Point2D<float> radian)
            => new(ToAngle_FromRadian(radian.X), ToAngle_FromRadian(radian.Y));

        /// <summary>
        ///     Converts the 3D angle coordinate from radian to degrees
        /// </summary>
        /// <param name="radian">
        ///     The 3D angle coordinate in radians
        /// </param>
        /// <returns>
        ///     The converted 3D angle coordinate in degrees
        /// </returns>
        public static Point3D<float> ToAngle_FromRadian(Point3D<float> radian)
            => new(ToAngle_FromRadian(radian.X), ToAngle_FromRadian(radian.Y), ToAngle_FromRadian(radian.Z));
    }
}
