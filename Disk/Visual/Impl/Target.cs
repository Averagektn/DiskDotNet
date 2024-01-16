using Disk.Data.Impl;
using System.Drawing;
using System.Windows.Media;

namespace Disk.Visual
{
    class Target(Point2D center, int radius, int speed, Brush color, Size iniSize) : Circle(center, radius, speed, color, iniSize)
    {
        public bool Contains(Point2D shot)
        {
            double distance = Math.Sqrt(Math.Pow((shot.X - Center.X) / Radius, 2) + Math.Pow((shot.Y - Center.Y) / Radius, 2));

            return distance <= 1.0f;
        }
    }
}
