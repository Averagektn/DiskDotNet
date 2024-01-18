using Disk.Data.Impl;

namespace Disk.Calculations.Impl
{
    // KAverage, 4 parts
    static class Classifier<CoordType> where CoordType : new()
    {
        public static IEnumerable<IEnumerable<Point2D<float>>> Classify(IEnumerable<Point2D<float>> dataset)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<IEnumerable<Point3D<float>>> Classify(IEnumerable<Point3D<float>> dataset)
        {
            throw new NotImplementedException();
        }
    }
}
