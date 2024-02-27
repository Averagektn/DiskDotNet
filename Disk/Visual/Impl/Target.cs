using Disk.Data.Impl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Disk.Visual.Impl
{
    /// <summary>
    ///     Represents a target with a center point, radius, and initial size
    /// </summary>
    /// <param name="center">
    ///     The center point of the target
    /// </param>
    /// <param name="radius">
    ///     The radius of the target
    /// </param>
    /// <param name="iniSize">
    ///     The initial size of the target
    /// </param>
    class Target(Point2D<int> center, int radius, Size iniSize) : User(center, radius, 0, Brushes.White, iniSize)
    {
        /// <summary>
        ///     Gets the maximum radius of the target
        /// </summary>
        public int MaxRadius => Radius * 5;

        /// <summary>
        ///     Gets the X-coordinate of the right edge of the circle
        /// </summary>
        public new int Right => Center.X + MaxRadius;

        /// <summary>
        ///     Gets the Y-coordinate of the top edge of the circle
        /// </summary>
        public new int Top => Center.Y - MaxRadius;

        /// <summary>
        ///     Gets the Y-coordinate of the bottom edge of the circle
        /// </summary>
        public new int Bottom => Center.Y + MaxRadius;

        /// <summary>
        ///     Gets the X-coordinate of the left edge of the circle
        /// </summary>
        public new int Left => Center.X - MaxRadius;

        /// <summary>
        ///     List of circles representing the concentric rings of the target
        /// </summary>
        protected readonly List<Circle> Circles =
            [
                new(center, radius * 5, 0, Brushes.Red, iniSize),
                new(center, radius * 4, 0, Brushes.White, iniSize),
                new(center, radius * 3, 0, Brushes.Red, iniSize),
                new(center, radius * 2, 0, Brushes.White, iniSize),
                new(center, radius * 1, 0, Brushes.Red, iniSize)
            ];

        /// <summary>
        ///     Draws the target by adding each circle to the specified UI element collection
        /// </summary>
        /// <param name="addChild">
        ///     The UI element collection to add the circles to
        /// </param>
        public override void Draw(IAddChild addChild)
        {
            foreach (var circle in Circles)
            {
                circle.Draw(addChild);
            }
        }

        /// <summary>
        ///     Moves the target in the specified directions
        /// </summary>
        /// <param name="moveTop">
        ///     Whether to move the target upwards
        /// </param>
        /// <param name="moveRight">
        ///     Whether to move the target to the right
        /// </param>
        /// <param name="moveBottom">
        ///     Whether to move the target downwards
        /// </param>
        /// <param name="moveLeft">
        ///     Whether to move the target to the left
        /// </param>
        public override void Move(bool moveTop, bool moveRight, bool moveBottom, bool moveLeft)
        {
            foreach (var circle in Circles)
            {
                circle.Move(moveTop, moveRight, moveBottom, moveLeft);
            }
        }

        /// <summary>
        ///     Scales the target to the specified size
        /// </summary>
        /// <param name="newSize">
        ///     The new size of the target
        /// </param>
        public override void Scale(Size newSize)
        {
            base.Scale(newSize);

            foreach (var circle in Circles)
            {
                circle.Scale(newSize);
            }
        }

        /// <summary>
        ///     Moves the target to the specified center point
        /// </summary>
        /// <param name="center">
        ///     The new center point of the target
        /// </param>
        public override void Move(Point2D<int> center)
        {
            Center = center;

            foreach (var circle in Circles)
            {
                circle.Move(center);
            }
        }

        /// <summary>
        ///     Receives a shot and calculates the score based on which circles of the target contain the shot
        /// </summary>
        /// <param name="shot">
        ///     The point of the shot
        /// </param>
        /// <returns>
        ///     The calculated score
        /// </returns>
        public override int ReceiveShot(Point2D<int> shot)
        {
            int res = 0;

            foreach (var circle in Circles)
            {
                res += circle.Contains(shot) ? 1 : 0;
            }

            return res;
        }

        /// <summary>
        ///     Determines if the target contains the specified point by checking if any of the circles contain the point
        /// </summary>
        /// <param name="shot">
        ///     The point to check
        /// </param>
        /// <returns>
        ///     True if the target contains the point, false otherwise
        /// </returns>
        public new bool Contains(Point2D<int> shot)
        {
            bool contains = false;

            foreach (var circle in Circles)
            {
                contains |= circle.Contains(shot);
            }

            return contains;
        }

        /// <summary>
        ///     Removes the target circles from the specified UI element collection
        /// </summary>
        /// <param name="collection">
        ///     The UI element collection to remove the circles from
        /// </param>
        public override void Remove(UIElementCollection collection)
        {
            foreach (var circle in Circles)
            {
                circle.Remove(collection);
            }
        }
    }
}
