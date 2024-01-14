using System.Drawing;

namespace Disk.Calculations
{
    static class Calculator
    {
        public static Point MathExp(IEnumerable<Point> dataset)
        {
            var res = MathExp(dataset.Select(p => new PointF(p.X, p.Y)));

            return new((int)Math.Round(res.X), (int)Math.Round(res.Y));
        }

        public static Point StandartDeviation(IEnumerable<Point> dataset)
        {
            var res = StandartDeviation(dataset.Select(p => new PointF(p.X, p.Y)));

            return new((int)Math.Round(res.X), (int)Math.Round(res.Y));
        }

        public static Point Dispersion(IEnumerable<Point> dataset)
        {
            var res = Dispersion(dataset.Select(p => new PointF(p.X, p.Y)));

            return new((int)Math.Round(res.X), (int)Math.Round(res.Y));
        }

        public static PointF MathExp(IEnumerable<PointF> dataset)
        {
            var x = dataset.Select(p => p.X);
            var y = dataset.Select(p => p.Y);

            return new(MathExp(x), MathExp(y));
        }

        public static PointF StandartDeviation(IEnumerable<PointF> dataset)
        {
            var x = dataset.Select(p => p.X);
            var y = dataset.Select(p => p.Y);

            return new(StandartDeviation(x), StandartDeviation(y));
        }

        public static PointF Dispersion(IEnumerable<PointF> dataset)
        {
            var x = dataset.Select(p => p.X);
            var y = dataset.Select(p => p.Y);

            return new(Dispersion(x), Dispersion(y));
        }

        private static float MathExp(IEnumerable<float> dataset)
        {
            throw new NotImplementedException();
        }

        private static float StandartDeviation(IEnumerable<float> dataset)
        {
            throw new NotImplementedException();
        }

        private static float Dispersion(IEnumerable<float> dataset)
        {
            throw new NotImplementedException();
        }
    }
}
