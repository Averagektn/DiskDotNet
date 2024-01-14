using System.Drawing;

namespace Disk.Calculations
{
    class Converter(int screenWidth, int screenHeight, float maxAngleX, float maxAngleY)
    {
        private readonly Point ScreenSize = new(screenWidth, screenHeight);
        private readonly PointF AngleSize = new(maxAngleX * 2, maxAngleY * 2);

        private readonly PointF MaxAngle = new(maxAngleX, maxAngleY);
        private readonly Point MaxLogCoord = new(screenWidth / 2, screenHeight / 2);

        // Window
        private int ToWndCoordX(float angle)
        {
            angle = -angle + MaxAngle.X;

            return (int)Math.Round(angle * ScreenSize.X / AngleSize.X);
        }

        private int ToWndCoordY(float angle)
        {
            angle = -angle + MaxAngle.Y;

            return (int)Math.Round(angle * ScreenSize.Y / AngleSize.Y);
        }

        private int ToWndCoordX(int logCoord)
        {
            return logCoord + MaxLogCoord.X;    
        }

        private int ToWndCoordY(int logCoord)
        {
            return MaxLogCoord.Y - logCoord;
        }

        public static Point ToWndCoord(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(int.Parse(coords[0]), int.Parse(coords[1]));
        }

        public Point ToWndCoord(PointF anglePoint)
        {
            return new(ToWndCoordX(anglePoint.X), ToWndCoordY(anglePoint.Y));
        }

        public Point ToWndCoord(Point logPoint)
        {
            return new(ToWndCoordX(logPoint.X), ToWndCoordY(logPoint.Y));
        }

        // Logical
        private int ToLogCoordX(int coord)
        {
            return coord + MaxLogCoord.X;
        }

        private int ToLogCoordY(int coord)
        {
            return MaxLogCoord.Y - coord;
        }

        private int ToLogCoordX(float angle)
        {
            return ToLogCoordX(ToWndCoordX(angle));
        }

        private int ToLogCoordY(float angle)
        {
            return ToLogCoordY(ToWndCoordY(angle));
        }

        public Point ToLogCoord(PointF point)
        {
            return new(ToLogCoordX(point.X), ToLogCoordY(point.Y));
        }

        public Point ToLogCoord(Point point)
        {
            return new(ToLogCoordX(point.X), ToLogCoordY(point.Y));
        }

        public static Point ToLogCoord(string str, char separator)
        {
            var coords = str.Split(separator);

            return new(int.Parse(coords[0]), int.Parse(coords[1]));
        }

        // ANGLES
        private float ToAngleX_FromWnd(int wndCoord)
        {
            return ToLogCoordX(wndCoord) * AngleSize.X / ScreenSize.X;
        }

        private float ToAngleY_FromWnd(int wndCoord)
        {
            return ToLogCoordY(wndCoord) * AngleSize.Y / ScreenSize.Y;
        }

        private float ToAngleX_FromLog(int logCoord)
        {
            return ToAngleX_FromWnd(logCoord + MaxLogCoord.X);
        }

        private float ToAngleY_FromLog(int logCoord)
        {
            return ToAngleY_FromWnd(logCoord + MaxLogCoord.Y);
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
