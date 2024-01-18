using Disk.Data.Impl;

namespace Disk.Calculations.Impl
{
    internal static class Calculator3D<PointType, CoordType>
        where PointType :
            Point3D<CoordType>,
            new()
        where CoordType :
            new()
    {
        public static PointType MathExp(IEnumerable<PointType> dataset)
        {
            var x = Calculator<CoordType>.MathExp(dataset.Select(p => p.X));
            var y = Calculator<CoordType>.MathExp(dataset.Select(p => p.Y));
            var z = Calculator<CoordType>.MathExp(dataset.Select(p => p.Z));

            return new PointType { X = x, Y = y, Z = z };
        }

        public static PointType StandartDeviation(IEnumerable<PointType> dataset)
        {
            var x = Calculator<CoordType>.StandartDeviation(dataset.Select(p => p.X));
            var y = Calculator<CoordType>.StandartDeviation(dataset.Select(p => p.Y));
            var z = Calculator<CoordType>.StandartDeviation(dataset.Select(p => p.Z));

            return new PointType { X = x, Y = y, Z = z };
        }

        public static PointType Dispersion(IEnumerable<PointType> dataset)
        {
            var x = Calculator<CoordType>.Dispersion(dataset.Select(p => p.X));
            var y = Calculator<CoordType>.Dispersion(dataset.Select(p => p.Y));
            var z = Calculator<CoordType>.Dispersion(dataset.Select(p => p.Z));

            return new PointType { X = x, Y = y, Z = z };
        }
    }
}
