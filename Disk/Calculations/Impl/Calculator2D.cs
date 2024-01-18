using Disk.Data.Impl;

namespace Disk.Calculations.Impl
{
    static class Calculator2D<PointType, CoordType>
        where PointType :
            Point2D<CoordType>,
            new()
        where CoordType :
            new()
    {
        public static PointType MathExp(IEnumerable<PointType> dataset)
        {
            var x = Calculator<CoordType>.MathExp(dataset.Select(p => p.X));
            var y = Calculator<CoordType>.MathExp(dataset.Select(p => p.Y));

            return new PointType { X = x, Y = y };
        }

        public static PointType StandartDeviation(IEnumerable<PointType> dataset)
        {
            var x = Calculator<CoordType>.StandartDeviation(dataset.Select(p => p.X));
            var y = Calculator<CoordType>.StandartDeviation(dataset.Select(p => p.Y));

            return new PointType { X = x, Y = y };
        }

        public static PointType Dispersion(IEnumerable<PointType> dataset)
        {
            var x = Calculator<CoordType>.Dispersion(dataset.Select(p => p.X));
            var y = Calculator<CoordType>.Dispersion(dataset.Select(p => p.Y));

            return new PointType { X = x, Y = y };
        }
    }
}
