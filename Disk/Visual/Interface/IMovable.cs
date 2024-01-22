using Disk.Data.Impl;

namespace Disk.Visual.Interface
{
    /// <summary>
    /// 
    /// </summary>
    interface IMovable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moveTop">
        /// 
        /// </param>
        /// <param name="moveRight">
        /// 
        /// </param>
        /// <param name="moveBottom">
        /// 
        /// </param>
        /// <param name="moveLeft">
        /// 
        /// </param>
        void Move(bool moveTop, bool moveRight, bool moveBottom, bool moveLeft);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center">
        /// 
        /// </param>
        void Move(Point2D<int> center);
    }
}
