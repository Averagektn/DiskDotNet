using Disk.Data.Impl;
using System.Drawing;

namespace Disk.Calculations
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
            ScreenSize = new(screenSize);
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
        private int ToWndCoordX(float angle)
        {
            angle = -angle + MaxAngle.Width;

            return (int)Math.Round(angle * ScreenSize.Width / AngleSize.Width);
        }

        private int ToWndCoordY(float angle)
        {
            angle = -angle + MaxAngle.Height;

            return (int)Math.Round(angle * ScreenSize.Height / AngleSize.Height);
        }

        private int ToWndCoordX(int logCoord)
        {
            return logCoord + MaxLogCoord.Width;
        }

        private int ToWndCoordY(int logCoord)
        {
            return MaxLogCoord.Height - logCoord;
        }

        public static Point2D<int> ToWndCoord(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(int.Parse(coords[0]), int.Parse(coords[1]));
        }

        public Point2D<int> ToWndCoord(PointF anglePoint)
        {
            return new(ToWndCoordX(anglePoint.X), ToWndCoordY(anglePoint.Y));
        }

        public Point2D<int> ToWndCoord(Point logPoint)
        {
            return new(ToWndCoordX(logPoint.X), ToWndCoordY(logPoint.Y));
        }

        // Logical
        private int ToLogCoordX(int coord)
        {
            return coord + MaxLogCoord.Width;
        }

        private int ToLogCoordY(int coord)
        {
            return MaxLogCoord.Height - coord;
        }

        private int ToLogCoordX(float angle)
        {
            return ToLogCoordX(ToWndCoordX(angle));
        }

        private int ToLogCoordY(float angle)
        {
            return ToLogCoordY(ToWndCoordY(angle));
        }

        public Point2D<int> ToLogCoord(PointF point)
        {
            return new(ToLogCoordX(point.X), ToLogCoordY(point.Y));
        }

        public Point2D<int> ToLogCoord(Point point)
        {
            return new(ToLogCoordX(point.X), ToLogCoordY(point.Y));
        }

        public static Point2D<int> ToLogCoord(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(int.Parse(coords[0]), int.Parse(coords[1]));
        }

        // ANGLES
        private float ToAngleX_FromWnd(int wndCoord)
        {
            return ToLogCoordX(wndCoord) * AngleSize.Width / ScreenSize.Width;
        }

        private float ToAngleY_FromWnd(int wndCoord)
        {
            return ToLogCoordY(wndCoord) * AngleSize.Height / ScreenSize.Height;
        }

        private float ToAngleX_FromLog(int logCoord)
        {
            return ToAngleX_FromWnd(logCoord + MaxLogCoord.Width);
        }

        private float ToAngleY_FromLog(int logCoord)
        {
            return ToAngleY_FromWnd(logCoord + MaxLogCoord.Height);
        }

        public static PointF ToAngle(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(float.Parse(coords[0]), float.Parse(coords[1]));
        }

        public PointF ToAngle_FromWnd(Point point)
        {
            return new(ToAngleX_FromWnd(point.X), ToAngleY_FromWnd(point.Y));
        }

        public PointF ToAngle_FromLog(Point logPoint)
        {
            return new(ToAngleX_FromLog(logPoint.X), ToAngleY_FromLog(logPoint.Y));
        }

        private static float ToAngle_FromRadian(float radian)
        {
            return (float)(radian * 180 / Math.PI);
        }

        public static PointF ToAngle_FromRadian(PointF radian)
        {
            return new(ToAngle_FromRadian(radian.X), ToAngle_FromRadian(radian.Y));
        }

        // Other
        public static float ToRadian_FromAngle(float angle)
        {
            return (float)(angle * Math.PI / 180);
        }

        public static PointF ToRadian_FromAngle(PointF angle)
        {
            return new(ToRadian_FromAngle(angle.X), ToRadian_FromAngle(angle.Y));
        }
    }
}
