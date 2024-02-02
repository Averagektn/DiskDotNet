using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Drawing;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Disk.Calculations.Impl
{
    /// <summary>
    ///     Provides conversion between coordinate systems
    /// </summary>
    class Converter : IScalable
    {
        /// <summary>
        ///     Size of the screen in angle space
        /// </summary>
        private readonly SizeF AngleSize;

        /// <summary>
        ///     Max angle size on both coordinate directions
        /// </summary>
        private readonly SizeF MaxAngle;

        /// <summary>
        ///     Size of the screen
        /// </summary>
        private Size ScreenSize;

        /// <summary>
        ///     Maximum size in logacal coordinates
        /// </summary>
        private Size MaxLogCoord;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Converter"/> class
        /// </summary>
        /// <param name="screenWidth">
        ///     The width of the screen
        /// </param>
        /// <param name="screenHeight">
        ///     The height of the screen
        /// </param>
        /// <param name="angleWidth">
        ///     The width of the angle
        /// </param>
        /// <param name="angleHeight">
        ///     The height of the angle
        /// </param>
        public Converter(int screenWidth, int screenHeight, float angleWidth, float angleHeight)
        {
            ScreenSize = new Size(screenWidth, screenHeight);
            AngleSize = new SizeF(angleWidth, angleHeight);
            MaxAngle = new SizeF(angleWidth / 2, angleHeight / 2);
            MaxLogCoord = new Size(screenWidth / 2, screenHeight / 2);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Converter"/> class
        /// </summary>
        /// <param name="screenSize">
        ///     The size of the screen
        /// </param>
        /// <param name="angleSize">
        ///     The size of the angle
        /// </param>
        public Converter(Point screenSize, PointF angleSize)
        {
            ScreenSize = new Size(ScreenSize.Width, ScreenSize.Height);
            MaxLogCoord = new Size(screenSize.X / 2, screenSize.Y / 2);

            AngleSize = new SizeF(angleSize);
            MaxAngle = new SizeF(angleSize.X / 2, angleSize.Y / 2);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Converter"/> class
        /// </summary>
        /// <param name="screenSize">
        ///     The size of the screen
        /// </param>
        /// <param name="angleSize">
        ///     The size of the angle
        /// </param>
        public Converter(Size screenSize, SizeF angleSize)
        {
            ScreenSize = screenSize;
            MaxLogCoord = new Size(screenSize.Width / 2, screenSize.Height / 2);

            AngleSize = angleSize;
            MaxAngle = new SizeF(angleSize.Width / 2, angleSize.Height / 2);
        }

        //
        // Window
        //

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

        //
        // Logical
        //

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

        //
        // ANGLES
        // 

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

        //
        // Other
        //

        /// <summary>
        ///     Converts the angle from degree to radian
        /// </summary>
        /// <param name="angle">
        ///     The angle in degrees
        /// </param>
        /// <returns>
        ///     The converted angle in radian
        /// </returns>
        public static float ToRadian_FromAngle(float angle) => (float)(angle * Math.PI / 180);

        /// <summary>
        ///     Converts the 2D angle coordinate from degree to radian
        /// </summary>
        /// <param name="angle">
        ///     The 2D angle coordinate in degrees
        /// </param>
        /// <returns>
        ///     The converted 2D angle coordinate in radian
        /// </returns>
        public static Point2D<float> ToRadian_FromAngle(Point2D<float> angle)
            => new(ToRadian_FromAngle(angle.X), ToRadian_FromAngle(angle.Y));

        /// <summary>
        ///     Converts the 3D angle coordinate from degree to radian
        /// </summary>
        /// <param name="angle">
        ///     The 3D angle coordinate in degrees
        /// </param>
        /// <returns>
        ///     The converted 3D angle coordinate in radian
        /// </returns>
        public static Point3D<float> ToRadian_FromAngle(Point3D<float> angle)
            => new(ToRadian_FromAngle(angle.X), ToRadian_FromAngle(angle.Y), ToRadian_FromAngle(angle.Z));

        /// <summary>
        ///     Scales the converter to a new screen size
        /// </summary>
        /// <param name="newSize">
        ///     The new screen size
        /// </param>
        public void Scale(Size newSize)
        {
            ScreenSize = newSize;
            MaxLogCoord = new(newSize.Width / 2, newSize.Height / 2);
        }
    }
}
