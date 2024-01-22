using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Drawing;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Disk.Calculations.Impl
{
    /// <summary>
    /// 
    /// </summary>
    class Converter : IScalable
    {
        private readonly SizeF AngleSize;
        private readonly SizeF MaxAngle;

        private Size ScreenSize;
        private Size MaxLogCoord;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="screenWidth">
        /// 
        /// </param>
        /// <param name="screenHeight">
        /// 
        /// </param>
        /// <param name="angleWidth">
        /// 
        /// </param>
        /// <param name="angleHeight">
        /// 
        /// </param>
        public Converter(int screenWidth, int screenHeight, float angleWidth, float angleHeight)
        {
            ScreenSize = new(screenWidth, screenHeight);
            AngleSize = new(angleWidth, angleHeight);
            MaxAngle = new(angleWidth / 2, angleHeight / 2);
            MaxLogCoord = new(screenWidth / 2, screenHeight / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="screenSize">
        /// 
        /// </param>
        /// <param name="angleSize">
        /// 
        /// </param>
        public Converter(Point screenSize, PointF angleSize)
        {
            ScreenSize = new(ScreenSize.Width, ScreenSize.Height);
            MaxLogCoord = new(screenSize.X / 2, screenSize.Y / 2);

            AngleSize = new(angleSize);
            MaxAngle = new(angleSize.X / 2, angleSize.Y / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="screenSize">
        /// 
        /// </param>
        /// <param name="angleSize">
        /// 
        /// </param>
        public Converter(Size screenSize, SizeF angleSize)
        {
            ScreenSize = screenSize;
            MaxLogCoord = new(screenSize.Width / 2, screenSize.Height / 2);

            AngleSize = angleSize;
            MaxAngle = new(angleSize.Width / 2, angleSize.Height / 2);
        }

        // Window
        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public int ToWndCoordX(float angle)
        {
            angle = angle + MaxAngle.Width;

            return (int)Math.Round(angle * ScreenSize.Width / AngleSize.Width);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public int ToWndCoordY(float angle)
        {
            angle = -angle + MaxAngle.Height;

            return (int)Math.Round(angle * ScreenSize.Height / AngleSize.Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logCoord">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public int ToWndCoordX(int logCoord) => (int)(logCoord + MaxLogCoord.Width);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logCoord">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public int ToWndCoordY(int logCoord) => (int)(MaxLogCoord.Height - logCoord);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">
        /// 
        /// </param>
        /// <param name="separator">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static Point2D<int> ToWndCoord(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(int.Parse(coords[0]), int.Parse(coords[1]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">
        /// 
        /// </param>
        /// <param name="separator">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static Point3D<int> ToWndCoord3D(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anglePoint">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<int> ToWndCoord(Point2D<float> anglePoint)
            => new(ToWndCoordX(anglePoint.X), ToWndCoordY(anglePoint.Y));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logPoint">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<int> ToWndCoord(Point2D<int> logPoint) => new(ToWndCoordX(logPoint.X), ToWndCoordY(logPoint.Y));

        // Logical
        /// <summary>
        /// 
        /// </summary>
        /// <param name="coord">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public int ToLogCoordX(int coord) => (int)(coord - MaxLogCoord.Width);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coord">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public int ToLogCoordY(int coord) => (int)(MaxLogCoord.Height - coord);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public int ToLogCoordX(float angle) => ToLogCoordX(ToWndCoordX(angle));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public int ToLogCoordY(float angle) => ToLogCoordY(ToWndCoordY(angle));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<int> ToLogCoord(Point2D<float> point) => new(ToLogCoordX(point.X), ToLogCoordY(point.Y));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<int> ToLogCoord(Point2D<int> point) => new(ToLogCoordX(point.X), ToLogCoordY(point.Y));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">
        /// 
        /// </param>
        /// <param name="separator">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static Point2D<int> ToLogCoord(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(int.Parse(coords[0]), int.Parse(coords[1]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">
        /// 
        /// </param>
        /// <param name="separator">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static Point3D<int> ToLogCoord3D(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]));
        }

        // ANGLES
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wndCoord">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public float ToAngleX_FromWnd(int wndCoord) => (float)(ToLogCoordX(wndCoord) * AngleSize.Width / ScreenSize.Width);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wndCoord">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public float ToAngleY_FromWnd(int wndCoord) => (float)(ToLogCoordY(wndCoord) * AngleSize.Height / ScreenSize.Height);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logCoord">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public float ToAngleX_FromLog(int logCoord) => ToAngleX_FromWnd((int)(logCoord + MaxLogCoord.Width));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logCoord">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public float ToAngleY_FromLog(int logCoord) => ToAngleY_FromWnd((int)(logCoord + MaxLogCoord.Height));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">
        /// 
        /// </param>
        /// <param name="separator">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static Point2D<float> ToAngle(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(float.Parse(coords[0]), float.Parse(coords[1]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">
        /// 
        /// </param>
        /// <param name="separator">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static Point3D<float> ToAngle3D(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<float> ToAngle_FromWnd(Point2D<int> point)
            => new(ToAngleX_FromWnd(point.X), ToAngleY_FromWnd(point.Y));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logPoint">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<float> ToAngle_FromLog(Point2D<int> logPoint)
            => new(ToAngleX_FromLog(logPoint.X), ToAngleY_FromLog(logPoint.Y));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radian">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static float ToAngle_FromRadian(float radian) => (float)(radian * 180 / Math.PI);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radian">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static Point2D<float> ToAngle_FromRadian(Point2D<float> radian)
            => new(ToAngle_FromRadian(radian.X), ToAngle_FromRadian(radian.Y));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radian">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static Point3D<float> ToAngle_FromRadian(Point3D<float> radian)
            => new(ToAngle_FromRadian(radian.X), ToAngle_FromRadian(radian.Y), ToAngle_FromRadian(radian.Z));

        // Other
        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static float ToRadian_FromAngle(float angle) => (float)(angle * Math.PI / 180);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static Point2D<float> ToRadian_FromAngle(Point2D<float> angle)
            => new(ToRadian_FromAngle(angle.X), ToRadian_FromAngle(angle.Y));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static Point3D<float> ToRadian_FromAngle(Point3D<float> angle)
            => new(ToRadian_FromAngle(angle.X), ToRadian_FromAngle(angle.Y), ToRadian_FromAngle(angle.Z));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newSize">
        /// 
        /// </param>
        public void Scale(Size newSize)
        {
            ScreenSize = newSize;
            MaxLogCoord = new(newSize.Width / 2, newSize.Height / 2);
        }
    }
}
