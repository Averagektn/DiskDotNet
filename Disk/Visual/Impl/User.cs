using Disk.Data.Impl;
using System.Windows;
using System.Windows.Media;

namespace Disk.Visual.Impl
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="center">
    /// 
    /// </param>
    /// <param name="radius">
    /// 
    /// </param>
    /// <param name="speed">
    /// 
    /// </param>
    /// <param name="color">
    /// 
    /// </param>
    /// <param name="iniSize">
    /// 
    /// </param>
    class User(Point2D<int> center, int radius, int speed, Brush color, Size iniSize) :
        Circle(center, radius, speed, color, iniSize)
    {
        /// <summary>
        /// 
        /// </summary>
        public event Action<Point2D<int>>? OnShot;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Point2D<int> Shot()
        {
            OnShot?.Invoke(Center);

            return Center;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shot">
        /// 
        /// </param>
        /// <returns>
        /// 
        public virtual int ReceiveShot(Point2D<int> shot) => Contains(shot) ? 5 : 0;
    }
}
