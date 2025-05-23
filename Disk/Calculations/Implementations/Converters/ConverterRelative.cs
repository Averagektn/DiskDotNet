﻿using Disk.Data.Impl;

namespace Disk.Calculations.Implementations.Converters;

/// <summary>
///     Provides conversion between coordinate systems
/// </summary>
public partial class Converter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="relativeCoord"></param>
    /// <returns></returns>
    public int ToWndX_FromRelative(float relativeCoord)
    {
        return (int)(relativeCoord * ScreenSize.Width);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="relativeCoord"></param>
    /// <returns></returns>
    public int ToWndY_FromRealtive(float relativeCoord)
    {
        return (int)(relativeCoord * ScreenSize.Height);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="relativeCoord"></param>
    /// <returns></returns>
    public Point2D<int> ToWnd_FromRelative(Point2D<float> relativeCoord)
    {
        return new(ToWndX_FromRelative(relativeCoord.X), ToWndY_FromRealtive(relativeCoord.Y));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="wndCoord"></param>
    /// <returns></returns>
    public float ToRelativeX_FromWnd(int wndCoord)
    {
        return (float)(wndCoord / ScreenSize.Width);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="wndCoord"></param>
    /// <returns></returns>
    public float ToRelativeY_FromWnd(int wndCoord)
    {
        return (float)(wndCoord / ScreenSize.Height);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="wndPoint"></param>
    /// <returns></returns>
    public Point2D<float> ToRelative_FromWnd(Point2D<int> wndPoint)
    {
        return new(ToRelativeX_FromWnd(wndPoint.X), ToRelativeY_FromWnd(wndPoint.Y));
    }
}
