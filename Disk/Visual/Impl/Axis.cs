using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Size = System.Windows.Size;

namespace Disk.Visual.Impl
{
    /// <summary>
    ///     Represents an axis that can be drawn and scaled
    /// </summary>
    class Axis : IDrawable, IScalable
    {
        /// <summary>
        ///     The initial size of the axis
        /// </summary>
        private readonly Size IniSize;

        /// <summary>
        ///     The line that represents the axis
        /// </summary>
        private readonly Line Line;

        /// <summary>
        ///     The start point of the axis
        /// </summary>
        private readonly Point2D<int> P1;

        /// <summary>
        ///     The end point of the axis
        /// </summary>
        private readonly Point2D<int> P2;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Axis"/> class
        /// </summary>
        /// <param name="p1">
        ///     The start point of the axis
        /// </param>
        /// <param name="p2">
        ///     The end point of the axis
        /// </param>
        /// <param name="currSize">
        ///     The current size of the axis
        /// </param>
        /// <param name="brush">
        ///     The brush used to draw the axis
        /// </param>
        public Axis(Point2D<int> p1, Point2D<int> p2, Size currSize, Brush brush)
        {
            P1 = p1;
            P2 = p2;

            Line = new Line()
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
        ///     Draws the axis and adds it as a child to the specified parent object
        /// </summary>
        /// <param name="addChild">
        ///     The parent object to add the axis to
        /// </param>
        public void Draw(IAddChild addChild) => addChild.AddChild(Line);

        /// <summary>
        ///     Removes the axis from the specified collection
        /// </summary>
        /// <param name="collection">
        ///     The collection to remove the axis from
        /// </param>
        public void Remove(UIElementCollection collection) => collection.Remove(Line);

        /// <summary>
        ///     Scales the axis to the specified size.
        /// </summary>
        /// <param name="newSize">
        ///     The new size to scale the axis to
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
