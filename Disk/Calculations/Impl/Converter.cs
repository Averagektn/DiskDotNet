using System.Drawing;

namespace Disk.Calculations
{
    class Converter
    {
        private readonly Point ScreenSize;
        private readonly Point LogSize;
        private readonly PointF AngleSize;

        private readonly PointF MaxAngle;
        private readonly Point MaxLogCoord;

        public Converter(int screenWidth, int screenHeight, float maxAngleX, float maxAngleY)
        {
            ScreenSize = new(screenWidth, screenHeight);
            LogSize = new(screenWidth, screenHeight);
            AngleSize = new(maxAngleX * 2, maxAngleY * 2);
            MaxAngle = new(maxAngleX, maxAngleY);
            MaxLogCoord = new(screenWidth, screenHeight);
        }

        // Window
        public int ToWndCoordX(float angle)
        {
            throw new NotImplementedException();
        }

        public int ToWndCoordY(float angle)
        {
            throw new NotImplementedException();
        }

        public int ToWndCoordX(int logCoord)
        {
            throw new NotImplementedException();
        }

        public int ToWndCoordY(int logCoord)
        {
            throw new NotImplementedException();
        }

        public static Point ToWndCoord(string str)
        {
            throw new NotImplementedException();
        }

        public Point ToWndCoord(PointF angles)
        {
            throw new NotImplementedException();
        }

        public Point ToWndCoord(Point logPoint)
        {
            throw new NotImplementedException();
        }

        public Point ToWndCoord_FromLogCoordString(string str)
        {
            throw new NotImplementedException();
        }

        public Point ToWndCoord_FromAngleString(string str)
        {
            throw new NotImplementedException();
        }

        public float ToWndCoord_FromRadian(float radian)
        {
            throw new NotImplementedException();
        }

        public Point ToWndCoord_FromRadian(PointF radian)
        {
            throw new NotImplementedException();
        }

        // ANGLES
        public float ToAngleX_FromWnd(int wndCoord)
        {
            throw new NotImplementedException();
        }

        public float ToAngleY_FromWnd(int wndCoord)
        {
            throw new NotImplementedException();
        }

        public float ToAngleX_FromLog(int logCoord)
        {
            throw new NotImplementedException();
        }

        public float ToAngleY_FromLog(int logCoord)
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

        public PointF ToAngle_FromLogCoordString(string str)
        {
            throw new NotImplementedException();
        }

        public PointF ToAngle_FromCoordString(string str)
        {
            throw new NotImplementedException();
        }

        public static float ToAngle_FromRadian(float radian)
        {
            throw new NotImplementedException();
        }

        public static PointF ToAngle_FromRadian(PointF radian)
        {
            throw new NotImplementedException();
        }

        // Logical
        public int ToLogCoordX(int coord)
        {
            throw new NotImplementedException();
        }

        public int ToLogCoordY(int coord)
        {
            throw new NotImplementedException();
        }

        public int ToLogCoordX_FromAngle(float angle)
        {
            throw new NotImplementedException();
        }

        public int ToLogCoordY_FromAngle(float angle)
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

        public Point ToLogCoord_FromCoordString(string str)
        {
            throw new NotImplementedException();
        }

        public Point ToLogCoord_FromAngleString(string str)
        {
            throw new NotImplementedException();
        }

        public float ToLogCoord_FromRadian(float radian)
        {
            throw new NotImplementedException();
        }

        public Point ToLogCoord_FromRadian(PointF radian)
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
