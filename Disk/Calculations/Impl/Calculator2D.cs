using Disk.Data.Impl;

namespace Disk.Calculations.Impl
{
    /// <summary>
    /// 
    /// </summary>
    static class Calculator2D
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataset">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static Point2D<float> MathExp(IEnumerable<Point2D<float>> dataset)
            => new
            (
                Calculator.MathExp(dataset.Select(p => p.X)),
                Calculator.MathExp(dataset.Select(p => p.Y))
            );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataset">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static Point2D<float> StandartDeviation(IEnumerable<Point2D<float>> dataset)
            => new
            (
                Calculator.StandartDeviation(dataset.Select(p => p.X)),
                Calculator.StandartDeviation(dataset.Select(p => p.Y))
            );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataset">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static Point2D<float> Dispersion(IEnumerable<Point2D<float>> dataset)
            => new
            (
                Calculator.Dispersion(dataset.Select(p => p.X)),
                Calculator.Dispersion(dataset.Select(p => p.Y))
            );
    }
}
