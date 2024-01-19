﻿using Disk.Data.Impl;

namespace Disk.Calculations.Impl
{
    static class Classifier<CoordType> where CoordType : IConvertible, new()
    {
        public static IEnumerable<IEnumerable<Point2D<CoordType>>> Classify(IEnumerable<Point2D<CoordType>> dataset,
            int classesCount)
        {
            var res = new List<List<Point2D<CoordType>>>(classesCount);
            var centers = GetInitialCenters(dataset, res, classesCount);
            bool isCounting = true;

            while (isCounting)
            {
                Separate(dataset, centers, res);

                isCounting = GenerateNewCenters2D(centers, res);
            }

            return res;
        }

        public static IEnumerable<IEnumerable<Point3D<CoordType>>> Classify(IEnumerable<Point3D<CoordType>> dataset,
            int classesCount)
        {
            var res = new List<List<Point3D<CoordType>>>(classesCount);
            var centers = GetInitialCenters(dataset, res, classesCount);
            bool isCounting = true;

            while (isCounting)
            {
                Separate(dataset, centers, res);

                isCounting = GenerateNewCenters3D(centers, res);
            }

            return res;
        }

        public static IEnumerable<IEnumerable<PolarPoint<CoordType>>> Classify(IEnumerable<PolarPoint<CoordType>> dataset,
            int classesCount)
        {
            var res = new List<List<PolarPoint<CoordType>>>(classesCount);
            var centers = GetInitialCenters(dataset, res, classesCount);
            bool isCounting = true;

            while (isCounting)
            {
                Separate(dataset, centers, res);

                isCounting = GenerateNewCenters2D(centers, res);
            }

            return res;
        }

        private static List<T> GetInitialCenters<T>(IEnumerable<T> dataset, IEnumerable<IEnumerable<T>> classification,
            int classesCount)
        {
            var res = new List<List<T>>(classesCount);
            var centers = new List<T>(classesCount);
            var random = new Random();

            for (int i = 0; i < classesCount; i++)
            {
                var p = dataset.ElementAt(random.Next(classesCount));

                if (!centers.Contains(p))
                {
                    res.Add([]);
                    res[i].Add(p);

                    centers.Add(p);
                }
                else
                {
                    i--;
                }
            }

            classification = res;

            return centers;
        }

        private static bool GenerateNewCenters2D<T>(IEnumerable<T> centers, IEnumerable<IEnumerable<T>> classification)
            where T :
                Point2D<CoordType>,
                new()
        {
            bool isCounting = false;

            for (int i = 0; i < centers.Count(); i++)
            {
                var avgX = (CoordType)Convert.ChangeType(classification.ElementAt(i).Average(p => p.XDbl), typeof(CoordType));
                var avgY = (CoordType)Convert.ChangeType(classification.ElementAt(i).Average(p => p.YDbl), typeof(CoordType));
                var newCenter = new T { X = avgX, Y = avgY };

                if (!newCenter.Equals(centers.ElementAt(i)))
                {
                    isCounting = true;
                    centers.ToList()[i] = newCenter;
                }

                classification.ToList()[i].ToList().Clear();
            }

            return isCounting;
        }

        private static bool GenerateNewCenters3D<T>(IEnumerable<T> centers, IEnumerable<IEnumerable<T>> classification)
            where T :
                Point3D<CoordType>,
                new()
        {
            bool isCounting = false;

            for (int i = 0; i < centers.Count(); i++)
            {
                var avgX = (CoordType)Convert.ChangeType(classification.ElementAt(i).Average(p => p.XDbl), typeof(CoordType));
                var avgY = (CoordType)Convert.ChangeType(classification.ElementAt(i).Average(p => p.YDbl), typeof(CoordType));
                var avgZ = (CoordType)Convert.ChangeType(classification.ElementAt(i).Average(p => p.ZDbl), typeof(CoordType));
                var newCenter = new T { X = avgX, Y = avgY, Z = avgZ };

                if (!newCenter.Equals(centers.ElementAt(i)))
                {
                    isCounting = true;
                    centers.ToList()[i] = newCenter;
                }

                classification.ToList()[i].ToList().Clear();
            }

            return isCounting;
        }

        private static void Separate<T>(IEnumerable<T> dataset, IEnumerable<T> centers,
            IEnumerable<IEnumerable<T>> classification) where T : Point2D<CoordType>
        {
            for (int i = 0; i < dataset.Count(); i++)
            {
                int classId = 0;
                var prevDistance = centers.ElementAt(classId).GetDistance(dataset.ElementAt(i));

                for (int j = 1; j < centers.Count(); j++)
                {
                    var currDistance = centers.ElementAt(j).GetDistance(dataset.ElementAt(i));

                    if (currDistance < prevDistance)
                    {
                        prevDistance = currDistance;
                        classId = j;
                    }
                }

                classification.ElementAt(classId).ToList().Add(dataset.ElementAt(i));
            }
        }
    }
}
