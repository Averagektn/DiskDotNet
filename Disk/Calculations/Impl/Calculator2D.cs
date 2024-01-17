using Disk.Data.Impl;

namespace Disk.Calculations
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
            var res = MathExp(dataset.Select(p => new PointType { X = p.X, Y = p.Y }));

            return new PointType { X = res.X, Y = res.Y };
        }

        public static PointType StandartDeviation(IEnumerable<PointType> dataset)
        {
            var res = StandartDeviation(dataset.Select(p => new PointType { X = p.X, Y = p.Y }));

            return new PointType { X = res.X, Y = res.Y };
        }

        public static PointType Dispersion(IEnumerable<PointType> dataset)
        {
            var res = Dispersion(dataset.Select(p => new PointType { X = p.X, Y = p.Y }));

            return new PointType { X = res.X, Y = res.Y };
        }

        public static CoordType MathExp(IEnumerable<CoordType> dataset)
        {
            throw new NotImplementedException();
        }

        public static CoordType StandartDeviation(IEnumerable<CoordType> dataset)
        {
            throw new NotImplementedException();
        }

        public static CoordType Dispersion(IEnumerable<CoordType> dataset)
        {
            throw new NotImplementedException();
        }
    }
}
