using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Size = System.Windows.Size;

namespace Disk.Visual.Impl
{
    /// <summary>
    ///     Represents a path that can be drawn and scaled
    /// </summary>
    class Path : IDrawable, IScalable
    {
        /// <summary>
        ///     The polyline used to draw the path
        /// </summary>
        private readonly Polyline Polyline;

        /// <summary>
        ///     The size of the angle
        /// </summary>
        private readonly SizeF AngleSize;

        /// <summary>
        ///     The list of points in the path
        /// </summary>
        private readonly List<Point2D<float>> Points = [];

        /// <summary>
        ///     The converter for coordinate transformations
        /// </summary>
        private Converter Converter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Path"/> class
        /// </summary>
        /// <param name="points">
        ///     The points in the path
        /// </param>
        /// <param name="currSize">
        ///     The current size of the path
        /// </param>
        /// <param name="angleSize">
        ///     The size of the angle
        /// </param>
        /// <param name="color">
        ///     The color of the path
        /// </param>
        public Path(IEnumerable<Point2D<float>> points, Size currSize, SizeF angleSize, Brush color)
        {
            AngleSize = angleSize;

            Converter = new(currSize, angleSize);

            Polyline = new Polyline()
            {
                Stroke = color,
                StrokeThickness = 3
            };

            foreach (var point in points)
            {
                Polyline.Points.Add(Converter.ToWndCoord(point).ToPoint());
                Points.Add(point);
            }
        }

        /// <summary>
        ///     Draws the path
        /// </summary>
        /// <param name="addChild">
        ///     The child element to add the path to
        /// </param>
        public void Draw(IAddChild addChild)
        {
            addChild.AddChild(Polyline);
        }

        /// <summary>
        ///     Removes the path from a UI element collection
        /// </summary>
        /// <param name="collection">
        ///     The UI element collection
        /// </param>
        public void Remove(UIElementCollection collection)
        {
            collection.Remove(Polyline);
        }

        /// <summary>
        ///     Scales the path to the specified size
        /// </summary>
        /// <param name="newSize">
        ///     The new size of the path
        /// </param>
        public void Scale(Size newSize)
        {
            Converter = new(newSize, AngleSize);

            Polyline.Points.Clear();

            foreach (var point in Points)
            {
                Polyline.Points.Add(Converter.ToWndCoord(point).ToPoint());
            }
        }
    }
}
