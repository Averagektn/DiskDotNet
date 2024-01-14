using System.Drawing;
using System.Windows.Media;

namespace Disk.Visual
{
    class User(Point center, int radius, int speed, Brush color, Point iniSize) : Circle(center, radius, speed, color, iniSize)
    {
        public event Action<object>? OnShot;

        public Point Shot()
        {
            OnShot?.Invoke(Center);

            return Center;
        }
    }
}
