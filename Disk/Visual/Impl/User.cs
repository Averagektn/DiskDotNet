using Disk.Data.Impl;
using System.Windows;
using System.Windows.Media;

namespace Disk.Visual.Impl
{
    /// <summary>
    ///     Represents a user with a center point, radius, speed, color, and initial size
    /// </summary>
    /// <param name="center">
    ///     The center point of the user
    /// </param>
    /// <param name="radius">
    ///     The radius of the user
    /// </param>
    /// <param name="speed">
    ///     The speed of the user
    /// </param>
    /// <param name="color">
    ///     The color of the user
    /// </param>
    /// <param name="iniSize">
    ///     The initial size of the user
    /// </param>
    class User(Point2D<int> center, int radius, int speed, Brush color, Size iniSize) :
        Circle(center, radius, speed, color, iniSize)
    {
        /// <summary>
        ///     Event that is triggered when the user shoots
        /// </summary>
        public event Action<Point2D<int>>? OnShot;

        public void ClearOnShot()
        {
            OnShot = null;
        }

        /// <summary>
        ///     Performs a shooting action and invokes the OnShot event
        /// </summary>
        /// <returns>
        ///     The center point of the user
        /// </returns>
        public Point2D<int> Shot()
        {
            OnShot?.Invoke(Center);

            return Center;
        }

        /// <summary>
        ///     Receives a shot and determines the score based on whether the shot is contained within the user
        /// </summary>
        /// <param name="shot">
        ///     The point of the shot
        /// </param>
        /// <returns>
        ///     The score. 5 if the shot is contained within the user, 0 otherwise
        /// </returns>
        public virtual int ReceiveShot(Point2D<int> shot) => Contains(shot) ? 5 : 0;
    }
}
