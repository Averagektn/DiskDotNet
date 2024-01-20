using Disk.Data.Impl;
using System.Drawing;
using System.Windows;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Disk.Calculations.Impl
{
    // REWORK
    class Converter
    {
        private readonly Size ScreenSize;
        private readonly SizeF AngleSize;
        private readonly SizeF MaxAngle;
        private readonly Size MaxLogCoord;

        public Converter(int screenWidth, int screenHeight, float angleWidth, float angleHeight)
        {
            ScreenSize = new(screenWidth, screenHeight);
            AngleSize = new(angleWidth, angleHeight);
            MaxAngle = new(angleWidth / 2, angleHeight / 2);
            MaxLogCoord = new(screenWidth / 2, screenHeight / 2);
        }

        public Converter(Point screenSize, PointF angleSize)
        {
            ScreenSize = new(ScreenSize.Width, ScreenSize.Height);
            MaxLogCoord = new(screenSize.X / 2, screenSize.Y / 2);

            AngleSize = new(angleSize);
            MaxAngle = new(angleSize.X / 2, angleSize.Y / 2);
        }

        public Converter(Size screenSize, SizeF angleSize)
        {
            ScreenSize = screenSize;
            MaxLogCoord = new(screenSize.Width / 2, screenSize.Height / 2);

            AngleSize = angleSize;
            MaxAngle = new(angleSize.Width / 2, angleSize.Height / 2);
        }

        // Window
        public int ToWndCoordX(float angle)
        {
            angle = -angle + MaxAngle.Width;

            return (int)Math.Round(angle * ScreenSize.Width / AngleSize.Width);
        }

        public int ToWndCoordY(float angle)
        {
            angle = -angle + MaxAngle.Height;

            return (int)Math.Round(angle * ScreenSize.Height / AngleSize.Height);
        }

        public int ToWndCoordX(int logCoord) => (int)(logCoord + MaxLogCoord.Width);

        public int ToWndCoordY(int logCoord) => (int)(MaxLogCoord.Height - logCoord);

        public static Point2D<int> ToWndCoord(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(int.Parse(coords[0]), int.Parse(coords[1]));
        }

        public static Point3D<int> ToWndCoord3D(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]));
        }

        public Point2D<int> ToWndCoord(Point2D<float> anglePoint)
            => new(ToWndCoordX(anglePoint.X), ToWndCoordY(anglePoint.Y));

        public Point2D<int> ToWndCoord(Point2D<int> logPoint) => new(ToWndCoordX(logPoint.X), ToWndCoordY(logPoint.Y));

        // Logical
        public int ToLogCoordX(int coord) => (int)(coord + MaxLogCoord.Width);

        public int ToLogCoordY(int coord) => (int)(MaxLogCoord.Height - coord);

        public int ToLogCoordX(float angle) => ToLogCoordX(ToWndCoordX(angle));

        public int ToLogCoordY(float angle) => ToLogCoordY(ToWndCoordY(angle));

        public Point2D<int> ToLogCoord(Point2D<float> point) => new(ToLogCoordX(point.X), ToLogCoordY(point.Y));

        public Point2D<int> ToLogCoord(Point2D<int> point) => new(ToLogCoordX(point.X), ToLogCoordY(point.Y));

        public static Point2D<int> ToLogCoord(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(int.Parse(coords[0]), int.Parse(coords[1]));
        }

        public static Point3D<int> ToLogCoord3D(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]));
        }

        // ANGLES
        public float ToAngleX_FromWnd(int wndCoord) => (float)(ToLogCoordX(wndCoord) * AngleSize.Width / ScreenSize.Width);

        public float ToAngleY_FromWnd(int wndCoord) => (float)(ToLogCoordY(wndCoord) * AngleSize.Height / ScreenSize.Height);

        public float ToAngleX_FromLog(int logCoord) => ToAngleX_FromWnd((int)(logCoord + MaxLogCoord.Width));

        public float ToAngleY_FromLog(int logCoord) => ToAngleY_FromWnd((int)(logCoord + MaxLogCoord.Height));

        public static Point2D<float> ToAngle(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(float.Parse(coords[0]), float.Parse(coords[1]));
        }

        public static Point3D<float> ToAngle3D(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));
        }

        public Point2D<float> ToAngle_FromWnd(Point2D<int> point)
            => new(ToAngleX_FromWnd(point.X), ToAngleY_FromWnd(point.Y));

        public Point2D<float> ToAngle_FromLog(Point2D<int> logPoint)
            => new(ToAngleX_FromLog(logPoint.X), ToAngleY_FromLog(logPoint.Y));

        public static float ToAngle_FromRadian(float radian) => (float)(radian * 180 / Math.PI);

        public static Point2D<float> ToAngle_FromRadian(Point2D<float> radian)
            => new(ToAngle_FromRadian(radian.X), ToAngle_FromRadian(radian.Y));

        public static Point3D<float> ToAngle_FromRadian(Point3D<float> radian)
            => new(ToAngle_FromRadian(radian.X), ToAngle_FromRadian(radian.Y), ToAngle_FromRadian(radian.Z));

        // Other
        public static float ToRadian_FromAngle(float angle) => (float)(angle * Math.PI / 180);

        public static Point2D<float> ToRadian_FromAngle(Point2D<float> angle)
            => new(ToRadian_FromAngle(angle.X), ToRadian_FromAngle(angle.Y));

        public static Point3D<float> ToRadian_FromAngle(Point3D<float> angle)
            => new(ToRadian_FromAngle(angle.X), ToRadian_FromAngle(angle.Y), ToRadian_FromAngle(angle.Z));
    }
}
