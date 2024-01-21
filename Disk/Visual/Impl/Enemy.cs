using Disk.Data.Impl;
using System.Windows;
using System.Windows.Media;

namespace Disk.Visual.Impl
{
    class Enemy(Point2D<int> center, int radius, int speed, Brush color, Size iniSize) : User(center, radius, speed, color, iniSize)
    {
        public void Follow(Point2D<int> target)
        {
            var direction = GetAngleDirection(target);

            if (!Contains(target))
            {
                bool moveRight = IsBetween(direction, 292.5f, 67.5f);
                bool moveTop = IsBetween(direction, 22.5f, 157.5f);
                bool moveLeft = IsBetween(direction, 112.5f, 247.5f);
                bool moveBottom = IsBetween(direction, 202.5f, 337.5f);

                Move(moveTop, moveRight, moveBottom, moveLeft);
            }
        }

        private float GetAngleDirection(Point2D<int> target)
        {
            var deltaX = target.X - Center.X;
            var deltaY = -target.Y + Center.Y;

            return (float)new PolarPoint<double>(new Point2D<double>(deltaX, deltaY)).Angle;
        }

        private static bool IsBetween(float angle, float left, float right)
        {
            angle = (angle + 360.0f) % 360.0f;
            left = (left + 360.0f) % 360.0f;
            right = (right + 360.0f) % 360.0f;

            if (left <= right)
            {
                return angle >= left && angle <= right;
            }
            else
            {
                return angle >= left || angle <= right;
            }
        }
    }
}
