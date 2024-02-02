﻿namespace Disk.Calculations.Impl
{
    /// <summary>
    ///     Calculates mathematical aspects of multiple coordinates
    /// </summary>
    static class Calculator
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
        public static float MathExp(IEnumerable<float> dataset) => dataset.Average();

        /// <summary>
        ///     Calculates standart deviation
        /// </summary>
        /// <param name="dataset">
        ///     Data to process
        /// </param>
        /// <returns>
        ///     Standart deviation
        /// </returns>
        public static float StandartDeviation(IEnumerable<float> dataset) => (float)Math.Sqrt(Dispersion(dataset));

        /// <summary>
        ///     Calculates dispersion
        /// </summary>
        /// <param name="dataset">
        ///     Data to process
        /// </param>
        /// <returns>
        ///     Dispesion
        /// </returns>
        public static float Dispersion(IEnumerable<float> dataset)
            => (float)dataset.Sum(x => Math.Pow(x - MathExp(dataset), 2)) / dataset.Count();
    }
}
