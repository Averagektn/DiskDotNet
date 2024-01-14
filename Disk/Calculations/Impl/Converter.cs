using System.Drawing;

namespace Disk.Calculations
{
    class Converter(int screenWidth, int screenHeight, float maxAngleX, float maxAngleY)
    {
        private readonly Point ScreenSize = new(screenWidth, screenHeight);
        private readonly Point LogSize = new(screenWidth, screenHeight);
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

        public static Point ToWndCoord(string str)
        {
            var coords = str.Split(' ');

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

        // ANGLES
        private float ToAngleX_FromWnd(int wndCoord)
        {
            throw new NotImplementedException();
        }

        private float ToAngleY_FromWnd(int wndCoord)
        {
            throw new NotImplementedException();
        }

        private float ToAngleX_FromLog(int logCoord)
        {
            throw new NotImplementedException();
        }

        private float ToAngleY_FromLog(int logCoord)
        {
            throw new NotImplementedException();
        }

        public PointF ToAngle(string str)
        {
            throw new NotImplementedException();
        }
        
        public PointF ToAngle_FromWnd(Point point)
        {
            throw new NotImplementedException();
        }

        public PointF ToAngle_FromLog(Point logPoint)
        {
            throw new NotImplementedException();
        }

        private static float ToAngle_FromRadian(float radian)
        {
            throw new NotImplementedException();
        }

        public static PointF ToAngle_FromRadian(PointF radian)
        {
            throw new NotImplementedException();
        }

        // Logical
        private int ToLogCoordX(int coord)
        {
            throw new NotImplementedException();
        }

        private int ToLogCoordY(int coord)
        {
            throw new NotImplementedException();
        }

        private int ToLogCoordX_FromAngle(float angle)
        {
            throw new NotImplementedException();
        }

        private int ToLogCoordY_FromAngle(float angle)
        {
            throw new NotImplementedException();
        }

        public Point ToLogCoord_FromAngle(PointF point)
        {
            throw new NotImplementedException();
        }

        public Point ToLogCoord_FromWnd(Point point)
        {
            throw new NotImplementedException();
        }

        public Point ToLogCoord(string str)
        {
            throw new NotImplementedException();
        }

        // Other
        public static float ToRadian_FromAngle(float angle)
        {
            throw new NotImplementedException();
        }

        public static PointF ToRadian_FromAngle(PointF angle)
        {
            throw new NotImplementedException();
        }

        public static object GetNumericValue(string str)
        {
            throw new NotImplementedException();
        }

        public static string ReplaceCommas(string str)
        {
            throw new NotImplementedException();
        }
    }
}
