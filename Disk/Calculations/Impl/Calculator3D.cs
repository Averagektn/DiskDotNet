using Disk.Data.Impl;

namespace Disk.Calculations.Impl
{
    /// <summary>
    ///     Calculates mathematical aspects of multiple 3D points
    /// </summary>
    static class Calculator3D
    {
        /// <summary>
        ///     Calculates mathematical expectation
        /// </summary>
        /// <param name="dataset">
        ///     Data to process
        /// </param>
        /// <returns>
        ///     Mathematical expectation
        /// </returns>
        public static Point3D<float> MathExp(IEnumerable<Point3D<float>> dataset)
        {
            var dataList = dataset.ToList();

            return new
            (
                Calculator.MathExp(dataList.Select(p => p.X)),
                Calculator.MathExp(dataList.Select(p => p.Y)),
                Calculator.MathExp(dataList.Select(p => p.Z))
            );
        }


        /// <summary>
        ///     Calculates standart deviation
        /// </summary>
        /// <param name="dataset">
        ///     Data to process
        /// </param>
        /// <returns>
        ///     Standart deviation
        /// </returns>
        public static Point3D<float> StandartDeviation(IEnumerable<Point3D<float>> dataset)
        {
            var dataList = dataset.ToList();

            return new
            (
                Calculator.StandartDeviation(dataList.Select(p => p.X)),
                Calculator.StandartDeviation(dataList.Select(p => p.Y)),
                Calculator.StandartDeviation(dataList.Select(p => p.Z))
            );
        }

        /// <summary>
        ///     Calculates dispersion
        /// </summary>
        /// <param name="dataset">
        ///     Data to process
        /// </param>
        /// <returns>
        ///     Dispesion
        /// </returns>
        public static Point3D<float> Dispersion(IEnumerable<Point3D<float>> dataset)
        {
            var dataList = dataset.ToList();

            return new
            (
                Calculator.Dispersion(dataList.Select(p => p.X)),
                Calculator.Dispersion(dataList.Select(p => p.Y)),
                Calculator.Dispersion(dataList.Select(p => p.Z))
            );
        }
    }
}
