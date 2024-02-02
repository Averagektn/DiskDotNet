﻿using Disk.Data.Impl;

namespace Disk.Calculations.Impl
{
    /// <summary>
    ///     Provides classification algorithms:<br/>
    ///     1. "K means" for non-polar points<br/>
    ///     2. Classification on angle value for polar points<br/>
    /// </summary>
    /// <typeparam name="CoordType">
    ///     Point coordiante type
    /// </typeparam>
    static class Classifier<CoordType> where CoordType : IConvertible, new()
    {
        /// <summary>
        ///     Classifies point by using "K means" algorithm for 2D point
        /// </summary>
        /// <param name="dataset">
        ///     Data to process
        /// </param>
        /// <param name="classesCount">
        ///     Required number of classes
        /// </param>
        /// <returns>
        ///     Classes wih points
        /// </returns>
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

        /// <summary>
        ///     Classifies point by using "K means" algorithm for 3D point
        /// </summary>
        /// <param name="dataset">
        ///     Data to process
        /// </param>
        /// <param name="classesCount">
        ///     Required number of classes
        /// </param>
        /// <returns>
        ///     Classes wih points
        /// </returns>
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

        /// <summary>
        ///     Classifies polar point by using it's angle value
        /// </summary>
        /// <param name="dataset">
        ///     Data to process
        /// </param>
        /// <param name="classesCount">
        ///     Required number of classes
        /// </param>
        /// <returns>
        ///     Classes wih points
        /// </returns>
        public static IEnumerable<IEnumerable<PolarPoint<CoordType>>> Classify(IEnumerable<PolarPoint<CoordType>> dataset,
            int classesCount)
        {
            double fullAngle = 360.0;
            var res = new List<List<PolarPoint<CoordType>>>(classesCount);
            var angleStep = fullAngle / classesCount;

            for (int i = 0; i < classesCount; i++)
            {
                res.Add([]);
            }

            for (int i = 0; i < dataset.Count(); i++)
            {
                res[(int)(dataset.ElementAt(i).Angle / angleStep)].Add(dataset.ElementAt(i));
            }

            return res;
        }

        /// <summary>
        ///     Get the initial centers for clustering.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of elements in the dataset
        /// </typeparam>
        /// <param name="dataset">
        ///     The dataset to be clustered
        /// </param>
        /// <param name="classification">
        ///     The initial classification of the dataset
        /// </param>
        /// <param name="classesCount">
        ///     The desired number of clusters
        /// </param>
        /// <returns>
        ///     A list of initial centers for clustering
        /// </returns>
        private static List<T> GetInitialCenters<T>(IEnumerable<T> dataset, IEnumerable<IEnumerable<T>> classification,
            int classesCount)
        {
            var res = new List<List<T>>(classesCount);
            var centers = new List<T>(classesCount);
            var random = new Random();
            dataset = dataset.ToList();

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

        /// <summary>
        ///     Generate new centers for 2D clustering.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of elements representing 2D points
        /// </typeparam>
        /// <param name="centers">
        ///     The current centers for clustering
        /// </param>
        /// <param name="classification">
        ///     The current classification of the dataset
        /// </param>
        /// <returns>
        ///     True if the centers have changed, false otherwise
        /// </returns>
        private static bool GenerateNewCenters2D<T>(IEnumerable<T> centers, IEnumerable<IEnumerable<T>> classification)
            where T : Point2D<CoordType>, new()
        {
            bool isCounting = false;
            var _centers = centers.ToList();
            var _classification = classification.ToList();

            for (int i = 0; i < centers.Count(); i++)
            {
                var _class = _classification[i].ToList();

                var avgX = (CoordType)Convert.ChangeType(_class.Average(p => p.XDbl), typeof(CoordType));
                var avgY = (CoordType)Convert.ChangeType(_class.Average(p => p.YDbl), typeof(CoordType));
                var newCenter = new T { X = avgX, Y = avgY };

                if (!newCenter.Equals(centers.ElementAt(i)))
                {
                    isCounting = true;
                    _centers[i] = newCenter;
                }

                _classification[i] = [];
            }

            classification = _classification;
            centers = _centers;

            return isCounting;
        }

        /// <summary>
        ///     Generate new centers for 3D clustering.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of elements representing 3D points
        /// </typeparam>
        /// <param name="centers">
        ///     The current centers for clustering
        /// </param>
        /// <param name="classification">
        ///     The current classification of the dataset
        /// </param>
        /// <returns>
        ///     True if the centers have changed, false otherwise
        /// </returns>
        private static bool GenerateNewCenters3D<T>(IEnumerable<T> centers, IEnumerable<IEnumerable<T>> classification)
            where T : Point3D<CoordType>, new()
        {
            bool isCounting = false;

            var _centers = centers.ToList();
            var _classification = classification.ToList();

            for (int i = 0; i < _centers.Count; i++)
            {
                var _class = _classification[i].ToList();

                var avgX = (CoordType)Convert.ChangeType(_class.Average(p => p.XDbl), typeof(CoordType));
                var avgY = (CoordType)Convert.ChangeType(_class.Average(p => p.YDbl), typeof(CoordType));
                var avgZ = (CoordType)Convert.ChangeType(_class.Average(p => p.ZDbl), typeof(CoordType));
                var newCenter = new T { X = avgX, Y = avgY, Z = avgZ };

                if (!newCenter.Equals(_centers[i]))
                {
                    isCounting = true;
                    _centers[i] = newCenter;
                }

                _classification[i] = [];
            }

            classification = _classification;
            centers = _centers;

            return isCounting;
        }

        /// <summary>
        ///     Separate the dataset into clusters based on the current centers.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of elements representing 2D points
        /// </typeparam>
        /// <param name="dataset">
        ///     The dataset to be clustered
        /// </param>
        /// <param name="centers">
        ///     The current centers for clustering
        /// </param>
        /// <param name="classification">
        ///     The classification of the dataset into clusters
        /// </param>
        private static void Separate<T>(IEnumerable<T> dataset, IEnumerable<T> centers,
            IEnumerable<IEnumerable<T>> classification) where T : Point2D<CoordType>
        {
            dataset = dataset.ToList();
            centers = centers.ToList();
            classification = classification.ToList();

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
