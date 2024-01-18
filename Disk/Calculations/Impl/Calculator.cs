namespace Disk.Calculations.Impl
{
    static class Calculator
    {
        public static float MathExp(IEnumerable<float> dataset) => dataset.Average();

        public static float StandartDeviation(IEnumerable<float> dataset) => (float)Math.Sqrt(Dispersion(dataset));

        public static float Dispersion(IEnumerable<float> dataset)
            => (float)dataset.Sum(x => Math.Pow(x - MathExp(dataset), 2)) / dataset.Count();
    }
}
