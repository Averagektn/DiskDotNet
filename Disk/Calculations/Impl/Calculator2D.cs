using Disk.Data.Impl;

namespace Disk.Calculations.Impl;

/// <summary>
///     Calculates mathematical aspects of multiple 2D points
/// </summary>
public static class Calculator2D
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
    public static Point2D<float> MathExp(IList<Point2D<float>> dataset)
    {
        return new
        (
            Calculator.MathExp([.. dataset.Select(p => p.X)]),
            Calculator.MathExp([.. dataset.Select(p => p.Y)])
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
    public static Point2D<float> StandartDeviation(IList<Point2D<float>> dataset)
    {
        return new
        (
            Calculator.StandartDeviation([.. dataset.Select(p => p.X)]),
            Calculator.StandartDeviation([.. dataset.Select(p => p.Y)])
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
    public static Point2D<float> Dispersion(IList<Point2D<float>> dataset)
    {
        return new
        (
            Calculator.Dispersion([.. dataset.Select(p => p.X)]),
            Calculator.Dispersion([.. dataset.Select(p => p.Y)])
        );
    }

    public static double CountShapeArea(IList<Point2D<float>> dataset, Point2D<float> center)
    {
        throw new NotImplementedException();
    }

    public static double[] GetAmplitudeCharacteristic(List<Point2D<float>> dataset)
    {
        throw new NotImplementedException();
    }
}
