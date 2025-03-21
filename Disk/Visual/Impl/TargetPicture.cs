﻿using Disk.Data.Impl;
using Disk.Visual.Interface;
using System.Windows;
using System.Windows.Controls;

namespace Disk.Visual.Impl;

/// <summary>
///     Same as <see cref="UserPicture"/>, but can receive shots
/// </summary>
public class TargetPicture : UserPicture, IProgressTarget
{
    /// <summary>
    ///     Invoked on <see cref="ReceiveShot(Point2D{int})"/> method call
    /// </summary>
    public event Action<int>? OnReceiveShot;

    /// <inheritdoc/>
    protected readonly double Hp;

    /// <inheritdoc/>
    public double Progress { get; protected set; }

    /// <inheritdoc/>
    public bool IsFull => Progress == Hp;

    /// <summary>
    ///     <inheritdoc/>
    /// </summary>
    /// <param name="filePath">
    ///     Path to image
    /// </param>
    /// <param name="center">
    ///     The center point of the target
    /// </param>
    /// <param name="speed">
    ///     The speed of the circle
    /// </param>
    /// <param name="imageSize">
    ///     Initial size of the image
    /// </param>
    /// <param name="parent">
    ///     Canvas, containing all figures
    /// </param>
    /// <param name="iniSize">
    ///     The initial size of the target
    /// </param>
    public TargetPicture(string imageFilePath, Point2D<int> center, Size imageSize, Canvas parent, Size iniSize, double hp)
        : base(imageFilePath, center, 0, imageSize, parent, iniSize)
    {
        Hp = hp;
    }

    /// <inheritdoc/>
    public virtual int ReceiveShot(Point2D<int> shot)
    {
        int res = Contains(shot) ? 1 : 0;

        Progress += res;

        OnReceiveShot?.Invoke(res);

        return res;
    }

    /// <inheritdoc/>
    public override bool Contains(Point2D<int> shot)
    {
        return Right <= shot.X && Left >= shot.X && Top >= shot.Y && Bottom <= shot.Y;
    }

    public void Reset()
    {
        Progress = 0;
    }
}
