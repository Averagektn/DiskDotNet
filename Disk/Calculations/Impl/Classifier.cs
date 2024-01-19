using Disk.Data.Impl;

namespace Disk.Calculations.Impl
{
    static class Classifier<CoordType> where CoordType : IConvertible, new()
    {
        public static IEnumerable<IEnumerable<Point2D<CoordType>>> Classify(IEnumerable<Point2D<CoordType>> dataset, 
            int classesCount)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<IEnumerable<Point3D<CoordType>>> Classify(IEnumerable<Point3D<CoordType>> dataset, 
            int classesCount)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<IEnumerable<PolarPoint<CoordType>>> Classify(IEnumerable<PolarPoint<CoordType>> dataset, 
            int classesCount)
        {
            throw new NotImplementedException();
        }
    }
}
