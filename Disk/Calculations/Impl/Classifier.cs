using Disk.Data.Impl;

namespace Disk.Calculations.Impl
{
    static class Classifier<CoordType> where CoordType : new()
    {
        public static IEnumerable<IEnumerable<Point2D<CoordType>>> Classify(IEnumerable<Point2D<CoordType>> dataset)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<IEnumerable<Point3D<CoordType>>> Classify(IEnumerable<Point3D<CoordType>> dataset)
        {
            throw new NotImplementedException();
        }
    }
}
