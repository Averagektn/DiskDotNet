using Disk.Data.Impl;
using System.Windows;
using System.Windows.Markup;
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
    /// <param name="iniSize">
    /// 
    /// </param>
    class Target(Point2D<int> center, int radius, Size iniSize) : User(center, radius, 0, Brushes.White, iniSize)
    {
        /// <summary>
        /// 
        /// </summary>
        public int MaxRadius => Radius * 5;

        private readonly List<Circle> Circles =
            [
                new(center, radius * 5, 0, Brushes.Red, iniSize),
                new(center, radius * 4, 0, Brushes.White, iniSize),
                new(center, radius * 3, 0, Brushes.Red, iniSize),
                new(center, radius * 2, 0, Brushes.White, iniSize),
                new(center, radius * 1, 0, Brushes.Red, iniSize)
            ];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addChild">
        /// 
        /// </param>
        public override void Draw(IAddChild addChild)
        {
            foreach (var circle in Circles)
            {
                circle.Draw(addChild);
            }
        }

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
        public override void Move(bool moveTop, bool moveRight, bool moveBottom, bool moveLeft)
        {
            foreach (var circle in Circles)
            {
                circle.Move(moveTop, moveRight, moveBottom, moveLeft);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newSize">
        /// 
        /// </param>
        public override void Scale(Size newSize)
        {
            foreach (var circle in Circles)
            {
                circle.Scale(newSize);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center">
        /// 
        /// </param>
        public override void Move(Point2D<int> center)
        {
            foreach (var circle in Circles)
            {
                circle.Move(center);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shot">
        /// 
        /// </param>
        /// <returns>
        /// 
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

        public new bool Contains(Point2D<int> shot)
        {
            bool contains = false;

            foreach (var circle in Circles)
            {
                contains |= circle.Contains(shot);
            }

            return contains;
        }
    }
}
