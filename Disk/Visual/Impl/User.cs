using Disk.Data.Impl;
using System.Windows;
using System.Windows.Media;

namespace Disk.Visual.Impl
{
    class User(Point2D<int> center, int radius, int speed, Brush color, Size iniSize) : Circle(center, radius, speed, color, iniSize)
    {
        public event Action<Point2D<int>>? OnShot;

        public Point2D<int> Shot()
        {
            OnShot?.Invoke(Center);

            return Center;
        }

        public virtual int ReceiveShot(Point2D<int> shot) => Contains(shot) ? 5 : 0;
    }
}
