using Disk.Data.Impl;

namespace Disk.Calculations.Impl;

/// <summary>
///     Calculates mathematical aspects of multiple 3D points
/// </summary>
public static class Calculator3D
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
    public static Point3D<float> MathExp(IList<Point3D<float>> dataset)
    {
        return new
        (
            Calculator.MathExp([.. dataset.Select(p => p.X)]),
            Calculator.MathExp([.. dataset.Select(p => p.Y)]),
            Calculator.MathExp([.. dataset.Select(p => p.Z)])
        );
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
    public static Point3D<float> StandartDeviation(IList<Point3D<float>> dataset)
    {
        return new
        (
            Calculator.StandartDeviation([.. dataset.Select(p => p.X)]),
            Calculator.StandartDeviation([.. dataset.Select(p => p.Y)]),
            Calculator.StandartDeviation([.. dataset.Select(p => p.Z)])
        );
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
    public static Point3D<float> Dispersion(IList<Point3D<float>> dataset)
    {
        return new
        (
            Calculator.Dispersion([.. dataset.Select(p => p.X)]),
            Calculator.Dispersion([.. dataset.Select(p => p.Y)]),
            Calculator.Dispersion([.. dataset.Select(p => p.Z)])
        );
    }
}
