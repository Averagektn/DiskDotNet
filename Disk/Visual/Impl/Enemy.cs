using Disk.Data.Impl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Disk.Visual.Impl;

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
/// <param name="parent">
///     Panel, containing all figures
/// </param>
/// <param name="iniSize">
///     The initial size of the enemy
/// </param>
public class Enemy(Point2D<int> center, int radius, int speed, Brush color, Panel parent, Size iniSize) :
    User(center, radius, speed, color, parent, iniSize)
{
    private const float TopRightRight = 22.5f;
    private const float TopRight = 67.5f;
    private const float TopLeft = 112.5f;
    private const float TopLeftLeft = 157.5f;
    private const float BottomLeftLeft = 202.5f;
    private const float BottomLeft = 247.5f;
    private const float BottomRight = 292.5f;
    private const float BottomRightRight = 337.5f;

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
            bool moveRight = IsBetween(direction, BottomRight, TopRight);
            bool moveTop = IsBetween(direction, TopRightRight, TopLeftLeft);
            bool moveLeft = IsBetween(direction, TopLeft, BottomLeft);
            bool moveBottom = IsBetween(direction, BottomLeftLeft, BottomRightRight);

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

        return left <= right ? angle >= left && angle <= right : angle >= left || angle <= right;
    }
}
