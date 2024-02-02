using Disk.Data.Impl;

namespace Disk.Calculations.Impl
{
    /// <summary>
    ///     Calculates mathematical aspects of multiple 2D points
    /// </summary>
    static class Calculator2D
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
        public static Point2D<float> MathExp(IEnumerable<Point2D<float>> dataset)
            => new
            (
                Calculator.MathExp(dataset.Select(p => p.X)),
                Calculator.MathExp(dataset.Select(p => p.Y))
            );

        /// <summary>
        ///     Calculates standart deviation
        /// </summary>
        /// <param name="dataset">
        ///     Data to process
        /// </param>
        /// <returns>
        ///     Standart deviation
        /// </returns>
        public static Point2D<float> StandartDeviation(IEnumerable<Point2D<float>> dataset)
            => new
            (
                Calculator.StandartDeviation(dataset.Select(p => p.X)),
                Calculator.StandartDeviation(dataset.Select(p => p.Y))
            );

        /// <summary>
        ///     Calculates dispersion
        /// </summary>
        /// <param name="dataset">
        ///     Data to process
        /// </param>
        /// <returns>
        ///     Dispesion
        /// </returns>
        public static Point2D<float> Dispersion(IEnumerable<Point2D<float>> dataset)
            => new
            (
                Calculator.Dispersion(dataset.Select(p => p.X)),
                Calculator.Dispersion(dataset.Select(p => p.Y))
            );
    }
}
