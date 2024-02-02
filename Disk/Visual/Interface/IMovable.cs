using Disk.Data.Impl;

namespace Disk.Visual.Interface
{
    /// <summary>
    ///     Represents an object that can be moved
    /// </summary>
    interface IMovable
    {
        /// <summary>
        ///     Moves the object in the specified direction
        /// </summary>
        /// <param name="moveTop">
        ///     A boolean indicating whether to move the object upwards
        /// </param>
        /// <param name="moveRight">
        ///     A boolean indicating whether to move the object to the right
        /// </param>
        /// <param name="moveBottom">
        ///     A boolean indicating whether to move the object downwards
        /// </param>
        /// <param name="moveLeft">
        ///     A boolean indicating whether to move the object to the left
        /// </param>
        void Move(bool moveTop, bool moveRight, bool moveBottom, bool moveLeft);

        /// <summary>
        ///     Moves the object to the specified center point
        /// </summary>
        /// <param name="center">
        ///     The new center point for the object
        /// </param>
        void Move(Point2D<int> center);
    }
}
