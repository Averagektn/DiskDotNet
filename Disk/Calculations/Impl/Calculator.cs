namespace Disk.Calculations.Impl
{
    /// <summary>
    /// 
    /// </summary>
    static class Calculator
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
        public static float MathExp(IEnumerable<float> dataset) => dataset.Average();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataset">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static float StandartDeviation(IEnumerable<float> dataset) => (float)Math.Sqrt(Dispersion(dataset));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataset">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static float Dispersion(IEnumerable<float> dataset)
            => (float)dataset.Sum(x => Math.Pow(x - MathExp(dataset), 2)) / dataset.Count();
    }
}
