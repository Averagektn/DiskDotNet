using Disk.Data.Impl;
using System.Windows;
using System.Windows.Media;

namespace Disk.Visual.Impl
{
    /// <summary>
    ///     Represents an enemy
    /// </summary>
    /// <param name="center">
    ///     The center point of the enemy
    /// </param>
    /// <param name="radius">
    ///     The radius of the enemy
    /// </param>
    /// <param name="speed">
    ///     The speed of the enemy
    /// </param>
    /// <param name="color">
    ///     The color of the enemy
    /// </param>
    /// <param name="iniSize">
    ///     The initial size of the enemy
    /// </param>
    class Enemy(Point2D<int> center, int radius, int speed, Brush color, Size iniSize) :
        User(center, radius, speed, color, iniSize)
    {
        /// <summary>
        ///     Makes the enemy follow the specified target point.
        /// </summary>
        /// <param name="target">
        ///     The target point to follow
        /// </param>
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

        /// <summary>
        ///     Calculates the angle direction between the enemy and the target point.
        /// </summary>
        /// <param name="target">
        ///     The target point
        /// </param>
        /// <returns>
        ///     The angle direction between the enemy and the target point
        /// </returns>
        private float GetAngleDirection(Point2D<int> target)
        {
            var deltaX = target.X - Center.X;
            var deltaY = Center.Y - target.Y;

            return (float)new PolarPoint<double>(new Point2D<double>(deltaX, deltaY)).Angle;
        }

        /// <summary>
        ///     Checks if the specified angle is between the left and right angles.
        /// </summary>
        /// <param name="angle">
        ///     The angle to check
        /// </param>
        /// <param name="left">
        ///     The left angle
        /// </param>
        /// <param name="right">
        ///     The right angle
        /// </param>
        /// <returns>
        ///     true if the angle is between the left and right angles, otherwise false
        /// </returns>
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
