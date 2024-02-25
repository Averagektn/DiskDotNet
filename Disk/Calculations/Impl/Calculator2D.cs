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
        public static Point2D<float> MathExp(IList<Point2D<float>> dataset) =>
            new
            (
                Calculator.MathExp(dataset.Select(p => p.X).ToList()),
                Calculator.MathExp(dataset.Select(p => p.Y).ToList())
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
        public static Point2D<float> StandartDeviation(IList<Point2D<float>> dataset) =>
            new
            (
                Calculator.StandartDeviation(dataset.Select(p => p.X).ToList()),
                Calculator.StandartDeviation(dataset.Select(p => p.Y).ToList())
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
        public static Point2D<float> Dispersion(IList<Point2D<float>> dataset) =>
            new
            (
                Calculator.Dispersion(dataset.Select(p => p.X).ToList()),
                Calculator.Dispersion(dataset.Select(p => p.Y).ToList())
            );
    }
}
