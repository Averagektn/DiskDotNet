namespace Disk.Calculations.Implementations;

/// <summary>
///     Calculates mathematical aspects of multiple coordinates
/// </summary>
public static class Calculator
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
    public static float MathExp(IList<float> dataset)
    {
        return dataset.Average();
    }

    /// <summary>
    ///     Rounds provided value to provided nearest
    /// </summary>
    /// <param name="value">
    ///     Value to round
    /// </param>
    /// <param name="nearest">
    ///     Nearest value to round to
    /// </param>
    /// <returns>
    ///     Rouded value
    /// </returns>
    public static int RoundToNearest(int value, int nearest)
    {
        return (int)(Math.Round((double)value / nearest) * nearest);
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
    public static float StandartDeviation(IList<float> dataset)
    {
        return float.Sqrt(Dispersion(dataset));
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
    public static float Dispersion(IList<float> dataset)
    {
        return (float)dataset.Sum(x => Math.Pow(x - MathExp(dataset), 2)) / dataset.Count;
    }
}
