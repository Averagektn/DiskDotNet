namespace Disk.Calculations.Impl
{
    static class Calculator
    {
        public static double MathExp(IEnumerable<double> dataset) => dataset.Average();

        public static double StandartDeviation(IEnumerable<double> dataset) => Math.Sqrt(Dispersion(dataset));

        public static double Dispersion(IEnumerable<double> dataset)
            => dataset.Sum(x => Math.Pow(x - MathExp(dataset), 2)) / dataset.Count();
    }
}
