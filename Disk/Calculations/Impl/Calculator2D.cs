using Disk.Data.Impl;

namespace Disk.Calculations.Impl
{
    static class Calculator2D
    {
        public static Point2D<double> MathExp(IEnumerable<Point2D<double>> dataset) => new
            (
                Calculator.MathExp(dataset.Select(p => p.X)),
                Calculator.MathExp(dataset.Select(p => p.Y))
            );

        public static Point2D<double> StandartDeviation(IEnumerable<Point2D<double>> dataset) => new
            (
                Calculator.StandartDeviation(dataset.Select(p => p.X)),
                Calculator.StandartDeviation(dataset.Select(p => p.Y))
            );

        public static Point2D<double> Dispersion(IEnumerable<Point2D<double>> dataset) => new
            (
                Calculator.Dispersion(dataset.Select(p => p.X)),
                Calculator.Dispersion(dataset.Select(p => p.Y))
            );
    }
}
