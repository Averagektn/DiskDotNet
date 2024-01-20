using Disk.Data.Impl;
using System.Windows;
using System.Windows.Media;

namespace Disk.Visual.Impl
{
    class User : Circle
    {
        public event Action<Point2D<int>>? OnShot;

        public User(Point2D<int> center, int radius, int speed, Brush color) : base(center, radius, speed, color) { }

        public User(Point2D<int> center, int radius, int speed, Brush color, Size iniSize) :
            base(center, radius, speed, color, iniSize)
        { }

        public Point2D<int> Shot()
        {
            OnShot?.Invoke(Center);

            return Center;
        }
    }
}
