using Disk.Data.Impl;

namespace Disk.Calculations
{
    static class Calculator
    {
        public static Point2D MathExp(IEnumerable<Point2D> dataset)
        {
            var res = MathExp(dataset.Select(p => new Point2DF(p.X, p.Y)));

            return new((int)Math.Round(res.X), (int)Math.Round(res.Y));
        }

        public static Point2D StandartDeviation(IEnumerable<Point2D> dataset)
        {
            var res = StandartDeviation(dataset.Select(p => new Point2DF(p.X, p.Y)));

            return new((int)Math.Round(res.X), (int)Math.Round(res.Y));
        }

        public static Point2D Dispersion(IEnumerable<Point2D> dataset)
        {
            var res = Dispersion(dataset.Select(p => new Point2DF(p.X, p.Y)));

            return new((int)Math.Round(res.X), (int)Math.Round(res.Y));
        }

        public static Point2DF MathExp(IEnumerable<Point2DF> dataset)
        {
            var x = dataset.Select(p => p.X);
            var y = dataset.Select(p => p.Y);

            return new(MathExp(x), MathExp(y));
        }

        public static Point2DF StandartDeviation(IEnumerable<Point2DF> dataset)
        {
            var x = dataset.Select(p => p.X);
            var y = dataset.Select(p => p.Y);

            return new(StandartDeviation(x), StandartDeviation(y));
        }

        public static Point2DF Dispersion(IEnumerable<Point2DF> dataset)
        {
            var x = dataset.Select(p => p.X);
            var y = dataset.Select(p => p.Y);

            return new(Dispersion(x), Dispersion(y));
        }

        public static Point3D MathExp(IEnumerable<Point3D> dataset)
        {
            var res = MathExp(dataset.Select(p => new Point3DF(p.X, p.Y, p.Z)));

            return new((int)Math.Round(res.X), (int)Math.Round(res.Y), (int)Math.Round(res.Z));
        }

        public static Point3D StandartDeviation(IEnumerable<Point3D> dataset)
        {
            var res = StandartDeviation(dataset.Select(p => new Point3DF(p.X, p.Y, p.Z)));

            return new((int)Math.Round(res.X), (int)Math.Round(res.Y), (int)Math.Round(res.Z));
        }

        public static Point3D Dispersion(IEnumerable<Point3D> dataset)
        {
            var res = Dispersion(dataset.Select(p => new Point3DF(p.X, p.Y, p.Z)));

            return new((int)Math.Round(res.X), (int)Math.Round(res.Y), (int)Math.Round(res.Z));
        }

        public static Point3DF MathExp(IEnumerable<Point3DF> dataset)
        {
            var x = dataset.Select(p => (float)p.X);
            var y = dataset.Select(p => (float)p.Y);
            var z = dataset.Select(p => (float)p.Z);

            return new(MathExp(x), MathExp(y), MathExp(z));
        }

        public static Point3DF StandartDeviation(IEnumerable<Point3DF> dataset)
        {
            var x = dataset.Select(p => (float)p.X);
            var y = dataset.Select(p => (float)p.Y);
            var z = dataset.Select(p => (float)p.Z);

            return new(StandartDeviation(x), StandartDeviation(y), StandartDeviation(z));
        }

        public static Point3DF Dispersion(IEnumerable<Point3DF> dataset)
        {
            var x = dataset.Select(p => (float)p.X);
            var y = dataset.Select(p => (float)p.Y);
            var z = dataset.Select(p => (float)p.Z);

            return new(Dispersion(x), Dispersion(y), Dispersion(z));
        }

        public static float MathExp(IEnumerable<float> dataset)
        {
            throw new NotImplementedException();
        }

        public static float StandartDeviation(IEnumerable<float> dataset)
        {
            throw new NotImplementedException();
        }

        public static float Dispersion(IEnumerable<float> dataset)
        { 
            throw new NotImplementedException();
        }
    }
}
