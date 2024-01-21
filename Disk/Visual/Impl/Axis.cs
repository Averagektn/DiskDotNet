using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Size = System.Windows.Size;

namespace Disk.Visual.Impl
{
    /// <summary>
    /// 
    /// </summary>
    class Axis : IDrawable, IScalable
    {
        private readonly Size IniSize;

        private readonly Line Line;

        private readonly Point2D<int> P1;
        private readonly Point2D<int> P2;

        private bool isDrawn = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1">
        /// 
        /// </param>
        /// <param name="p2">
        /// 
        /// </param>
        /// <param name="currSize">
        /// 
        /// </param>
        /// <param name="brush">
        /// 
        /// </param>
        public Axis(Point2D<int> p1, Point2D<int> p2, Size currSize, Brush brush)
        {
            P1 = p1;
            P2 = p2;

            Line = new()
            {
                X1 = p1.X,
                Y1 = p1.Y,
                X2 = p2.X,
                Y2 = p2.Y,
                Stroke = brush
            };

            IniSize = currSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addChild">
        /// 
        /// </param>
        public void Draw(IAddChild addChild)
        {
            if (!isDrawn)
            {
                isDrawn = true;

                addChild.AddChild(Line);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newSize">
        /// 
        /// </param>
        public void Scale(Size newSize)
        {
            var xScale = newSize.Width / IniSize.Width;
            var yScale = newSize.Height / IniSize.Height;

            Line.X1 = P1.X * xScale;
            Line.X2 = P2.X * xScale;
            Line.Y1 = P1.Y * yScale;
            Line.Y2 = P2.Y * yScale;
        }
    }
}
