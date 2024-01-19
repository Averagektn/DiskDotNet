﻿using Disk.Data.Impl;

namespace Disk.Calculations.Impl
{
    static class Calculator3D
    {
        public static Point3D<float> MathExp(IEnumerable<Point3D<float>> dataset) => new
            (
                Calculator.MathExp(dataset.Select(p => p.X)), 
                Calculator.MathExp(dataset.Select(p => p.Y)), 
                Calculator.MathExp(dataset.Select(p => p.Z))
            );

        public static Point3D<float> StandartDeviation(IEnumerable<Point3D<float>> dataset) => new
            (
                Calculator.StandartDeviation(dataset.Select(p => p.X)),
                Calculator.StandartDeviation(dataset.Select(p => p.Y)),
                Calculator.StandartDeviation(dataset.Select(p => p.Z))
            );

        public static Point3D<float> Dispersion(IEnumerable<Point3D<float>> dataset) => new
            (
                Calculator.Dispersion(dataset.Select(p => p.X)),
                Calculator.Dispersion(dataset.Select(p => p.Y)),
                Calculator.Dispersion(dataset.Select(p => p.Z))
            );
    }
}