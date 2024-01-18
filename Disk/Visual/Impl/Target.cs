using Disk.Data.Impl;
using System.Drawing;
using System.Windows.Media;

namespace Disk.Visual.Impl
{
    class Target(Point2D<int> center, int radius, int speed, Brush color, Size iniSize) : Circle(center, radius, speed, color, iniSize)
    {
        public event Action<Point2D<int>>? OnShot;

        public Point2D<int> Shot()
        {
            OnShot?.Invoke(Center);

            return Center;
        }

        public bool Contains(Point2D<int> p)
            => Math.Sqrt(Math.Pow((p.X - Center.X) / Radius, 2) + Math.Pow((p.Y - Center.Y) / Radius, 2)) <= 1.0f;
    }
}
