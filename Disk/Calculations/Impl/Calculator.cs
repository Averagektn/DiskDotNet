namespace Disk.Calculations.Impl;

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
