using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Disk.Calculations.Implementations.Converters;
using Disk.Data.Impl;
using Disk.Visual.Implementations;
using Disk.Visual.Interfaces;

using Settings = Disk.Properties.Config.Config;

namespace Disk.Services.Implementations;

public static class DrawableFabric
{
    private static Settings Settings => Settings.Default;
    private static Size ScreenIniSize => new(Settings.IniScreenWidth, Settings.IniScreenHeight);
    private static int ScreenIniCenterX => (int)ScreenIniSize.Width / 2;
    private static int ScreenIniCenterY => (int)ScreenIniSize.Height / 2;
    private static Brush CursorBrush =>
        new SolidColorBrush(Color.FromRgb(Settings.CursorColor.R, Settings.CursorColor.G, Settings.CursorColor.B));
    private static float XAngleSize => Settings.XMaxAngle * 2;
    private static float YAngleSize => Settings.YMaxAngle * 2;
    private static int TargetHp => Settings.TargetHp;

    public static Cursor GetIniCursor(string cursorImagePath, Panel parent)
    {
        return File.Exists(cursorImagePath)
            ? new CursorPicture
            (
                filePath: cursorImagePath,
                center: new(ScreenIniCenterX, ScreenIniCenterY),
                speed: 0,
                imageSize: new(Settings.IniCursorRadius * 10, Settings.IniCursorRadius * 10),
                parent,
                iniSize: ScreenIniSize
            )
            : new Cursor
        (
            center: new(ScreenIniCenterX, ScreenIniCenterY),
            radius: Settings.IniCursorRadius * 5,
            speed: 0,
            color: CursorBrush,
            parent,
            iniSize: ScreenIniSize
        );
    }

    public static IProgressTarget GetIniProgressTarget(string targetImagePath, Point2D<int> center, Panel parent)
    {
        return File.Exists(targetImagePath)
            ? new TargetPicture
            (
                imageFilePath: targetImagePath,
                center,
                imageSize: new(Settings.IniTargetRadius * 10, Settings.IniTargetRadius * 10),
                parent,
                iniSize: ScreenIniSize,
                hp: Settings.TargetHp
            )
            : new ProgressTarget(center, radius: Settings.IniTargetRadius * 6, parent, TargetHp, iniSize: ScreenIniSize);
    }

    public static Converter GetIniConverter()
    {
        return new(screenSize: ScreenIniSize, angleSize: new(XAngleSize, YAngleSize));
    }
}
