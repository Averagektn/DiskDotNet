using Disk.Data.Impl;

namespace Disk.Calculations.Impl
{
    static class Classifier<CoordType> where CoordType : IConvertible, new()
    {
        public static IEnumerable<IEnumerable<Point2D<CoordType>>> Classify(IEnumerable<Point2D<CoordType>> dataset,
            int classesCount)
        {
            var res = new List<List<Point2D<CoordType>>>(classesCount);
            var centers = new List<Point2D<CoordType>>(classesCount);
            var random = new Random();

            for (int i = 0; i < classesCount; i++)
            {
                res.Add([]);
            }

            for (int i = 0; i < classesCount; i++)
            {
                var p = dataset.ElementAt(random.Next(classesCount));

                if (!centers.Contains(p))
                {
                    centers.Add(p);
                    res[i].Add(p);
                }
                else
                {
                    i--;
                }
            }

            bool isCounting = true;

            while (isCounting)
            {
                for (int i = 0; i < dataset.Count(); i++)
                {
                    int classId = 0;
                    var prevDistance = centers[classId].GetDistance(dataset.ElementAt(i));

                    for (int j = 1; j < classesCount; j++)
                    {
                        var currDistance = centers[j].GetDistance(dataset.ElementAt(i));

                        if (currDistance < prevDistance)
                        {
                            prevDistance = currDistance;
                            classId = j;
                        }
                    }

                    res[classId].Add(dataset.ElementAt(i));
                }

                isCounting = false;

                for (int i = 0; i < classesCount; i++)
                {
                    var avgX = (CoordType)Convert.ChangeType(res[i].Average(p => p.XDbl), typeof(CoordType));
                    var avgY = (CoordType)Convert.ChangeType(res[i].Average(p => p.YDbl), typeof(CoordType));
                    var newCenter = new Point2D<CoordType>(avgX, avgY);

                    if (!newCenter.Equals(centers.ElementAt(i)))
                    {
                        isCounting = true;
                        centers[i] = newCenter;
                    }

                    res[i].Clear();
                }

            }

            return res;
        }

        public static IEnumerable<IEnumerable<Point3D<CoordType>>> Classify(IEnumerable<Point3D<CoordType>> dataset,
            int classesCount)
        {
            var res = new List<List<Point3D<CoordType>>>(classesCount);
            var centers = new List<Point3D<CoordType>>(classesCount);
            var random = new Random();

            for (int i = 0; i < classesCount; i++)
            {
                res.Add([]);
            }

            for (int i = 0; i < classesCount; i++)
            {
                var p = dataset.ElementAt(random.Next(classesCount));

                if (!centers.Contains(p))
                {
                    centers.Add(p);
                    res[i].Add(p);
                }
                else
                {
                    i--;
                }
            }

            bool isCounting = true;

            while (isCounting)
            {
                for (int i = 0; i < dataset.Count(); i++)
                {
                    int classId = 0;
                    var prevDistance = centers[classId].GetDistance(dataset.ElementAt(i));

                    for (int j = 1; j < classesCount; j++)
                    {
                        var currDistance = centers[j].GetDistance(dataset.ElementAt(i));

                        if (currDistance < prevDistance)
                        {
                            prevDistance = currDistance;
                            classId = j;
                        }
                    }

                    res[classId].Add(dataset.ElementAt(i));
                }

                isCounting = false;

                for (int i = 0; i < classesCount; i++)
                {
                    var avgX = (CoordType)Convert.ChangeType(res[i].Average(p => p.XDbl), typeof(CoordType));
                    var avgY = (CoordType)Convert.ChangeType(res[i].Average(p => p.YDbl), typeof(CoordType));
                    var avgZ = (CoordType)Convert.ChangeType(res[i].Average(p => p.ZDbl), typeof(CoordType));
                    var newCenter = new Point3D<CoordType>(avgX, avgY, avgZ);

                    if (!newCenter.Equals(centers.ElementAt(i)))
                    {
                        isCounting = true;
                        centers[i] = newCenter;
                    }

                    res[i].Clear();
                }
            }

            return res;
        }

        public static IEnumerable<IEnumerable<PolarPoint<CoordType>>> Classify(IEnumerable<PolarPoint<CoordType>> dataset,
            int classesCount)
        {
            var res = new List<List<PolarPoint<CoordType>>>(classesCount);
            var centers = new List<PolarPoint<CoordType>>(classesCount);
            var random = new Random();

            for (int i = 0; i < classesCount; i++)
            {
                res.Add([]);
            }

            for (int i = 0; i < classesCount; i++)
            {
                var p = dataset.ElementAt(random.Next(classesCount));

                if (!centers.Contains(p))
                {
                    centers.Add(p);
                    res[i].Add(p);
                }
                else
                {
                    i--;
                }
            }

            bool isCounting = true;

            while (isCounting)
            {
                for (int i = 0; i < dataset.Count(); i++)
                {
                    int classId = 0;
                    var prevDistance = centers[classId].GetDistance(dataset.ElementAt(i));

                    for (int j = 1; j < classesCount; j++)
                    {
                        var currDistance = centers[j].GetDistance(dataset.ElementAt(i));

                        if (currDistance < prevDistance)
                        {
                            prevDistance = currDistance;
                            classId = j;
                        }
                    }

                    res[classId].Add(dataset.ElementAt(i));
                }

                isCounting = false;

                for (int i = 0; i < classesCount; i++)
                {
                    var avgX = (CoordType)Convert.ChangeType(res[i].Average(p => p.XDbl), typeof(CoordType));
                    var avgY = (CoordType)Convert.ChangeType(res[i].Average(p => p.YDbl), typeof(CoordType));
                    var newCenter = new PolarPoint<CoordType>(avgX, avgY);

                    if (!newCenter.Equals(centers.ElementAt(i)))
                    {
                        isCounting = true;
                        centers[i] = newCenter;
                    }

                    res[i].Clear();
                }
            }

            return res;
        }
    }
}
