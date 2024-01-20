using Disk.Calculations.Impl;
using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Size = System.Windows.Size;

namespace Disk.Visual.Impl
{
    class Graph : IDrawable, IScalable
    {
        private readonly Polygon Polygon;

        private readonly IEnumerable<int> Frequency;

        private readonly int SegmentsNum;

        private int Radius;

        public Graph(IEnumerable<PolarPoint<float>> points, Size currSize, Brush color, int segmentsNum = 4)
        {
            SegmentsNum = segmentsNum;

            Radius = (int)Math.Min(currSize.Width / 2, currSize.Height / 2);

            Frequency = GetFrequency(Classifier<float>.Classify(points, segmentsNum));

            Polygon = new()
            {
                Fill = color
            };
        }

        public void Draw(IAddChild addChild)
        {
            FillPolygon();

            addChild.AddChild(Polygon);
        }

        public void Scale(Size newSize)
        {
            Radius = (int)Math.Min(newSize.Width, newSize.Height);
            Polygon.Points.Clear();

            FillPolygon();
        }

        private void FillPolygon()
        {
            var angleStep = 360.0 / SegmentsNum;
            var maxFrequency = Frequency.Max();

            int i = 0;
            for (var angle = angleStep / 2; angle < 360.0; angle += angleStep, i++)
            {
                var radius = Radius * (double)maxFrequency / Frequency.ElementAt(i);

                var point = new PolarPoint<float>(radius, angle);

                Polygon.Points.Add(point.ToPoint());
            }
        }

        private static List<int> GetFrequency(IEnumerable<IEnumerable<PolarPoint<float>>> dataset)
        {
            var res = new List<int>(dataset.Count());

            foreach (var points in dataset)
            {
                res.Add(points.Count());
            }

            return res;
        }
    }
}
