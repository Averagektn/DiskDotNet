using Disk.Calculations.Impl;
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
    ///     Represents a graph that can be drawn and scaled
    /// </summary>
    class Graph : IDrawable, IScalable
    {
        /// <summary>
        ///     The size of the graph
        /// </summary>
        private Size Size;

        /// <summary>
        ///     The polygon used to draw the graph
        /// </summary>
        private readonly Polygon Polygon;

        /// <summary>
        ///     The frequency of data points
        /// </summary>
        private readonly IEnumerable<int> Frequency;

        /// <summary>
        ///     The number of segments in the graph
        /// </summary>
        private readonly int SegmentsNum;

        /// <summary>
        ///     The radius of the graph
        /// </summary>
        private int Radius;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Graph"/> class
        /// </summary>
        /// <param name="points">
        ///     The data points in polar coordinates
        /// </param>
        /// <param name="currSize">
        ///     The current size of the graph
        /// </param>
        /// <param name="color">
        ///     The color of the graph
        /// </param>
        /// <param name="segmentsNum">
        ///     The number of segments in the graph. Default is 4
        /// </param>
        public Graph(IEnumerable<PolarPoint<float>> points, Size currSize, Brush color, int segmentsNum = 4)
        {
            SegmentsNum = segmentsNum;
            Size = currSize;
            Radius = (int)(Math.Min(Size.Width, Size.Height) * 0.9) / 2;

            Frequency = GetFrequency(Classifier<float>.Classify(points.ToList(), segmentsNum));

            Polygon = new()
            {
                Fill = color
            };
        }

        /// <summary>
        ///     Draws the graph
        /// </summary>
        /// <param name="addChild">
        ///     The child element to add the graph to
        /// </param>
        public void Draw(IAddChild addChild)
        {
            FillPolygon();

            addChild.AddChild(Polygon);
        }

        /// <summary>
        ///     Scales the graph to the specified size
        /// </summary>
        /// <param name="newSize">
        ///     The new size of the graph
        /// </param>
        public void Scale(Size newSize)
        {
            Size = newSize;

            Radius = (int)(Math.Min(Size.Width, Size.Height) * 0.9) / 2;

            Polygon.Points.Clear();

            FillPolygon();
        }

        /// <summary>
        ///     Fills the polygon with data points
        /// </summary>
        private void FillPolygon()
        {
            var angleStep = 360.0 / SegmentsNum;
            var maxFrequency = Frequency.Max();

            int i = 0;

            for (var angle = angleStep / 2; angle < 360.0; angle += angleStep, i++)
            {
                var radius = Radius * Frequency.ElementAt(i) / (double)maxFrequency;
                //REMOVE
                if (radius < 5)
                {
                    radius = 50;
                }
                //

                var point = new PolarPoint<float>(radius, Math.PI * angle / 180);

                Polygon.Points.Add(new(Size.Width / 2 + point.X, Size.Height / 2 - point.Y));
            }
        }

        /// <summary>
        ///     Gets the frequency of data points in each segment
        /// </summary>
        /// <param name="dataset">
        ///     The dataset of data points
        /// </param>
        /// <returns>
        ///     A list of frequencies of data points
        /// </returns>
        private static List<int> GetFrequency(IEnumerable<IEnumerable<PolarPoint<float>>> dataset)
        {
            var res = new List<int>(dataset.Count());

            foreach (var points in dataset)
            {
                res.Add(points.Count());
            }

            return res;
        }

        /// <summary>
        ///     
        /// s the graph from a UI element collection
        /// </summary>
        /// <param name="collection">
        ///     The UI element collection
        /// </param>
        public void Remove(UIElementCollection collection)
        {
            collection.Remove(Polygon);
        }
    }
}
