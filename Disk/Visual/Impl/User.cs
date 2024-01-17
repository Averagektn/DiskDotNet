using Disk.Data.Impl;
using System.Drawing;
using System.Windows.Media;

namespace Disk.Visual
{
    class User(Point2D<int> center, int radius, int speed, Brush color, Size iniSize) : Circle(center, radius, speed, color, iniSize)
    {
        public event Action<object>? OnShot;

        public Point2D<int> Shot()
        {
            OnShot?.Invoke(Center);

            return Center;
        }
    }
}
