using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Visual.Impl;
using Disk.Visual.Interface;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Settings = Disk.Properties.Config.Config;

namespace Disk.Service.Implementation;

public static class DrawableFabric
{
    private static Settings Settings => Settings.Default;
    private static Size ScreenIniSize => new(Settings.IniScreenWidth, Settings.IniScreenHeight);
    private static int ScreenIniCenterX => (int)ScreenIniSize.Width / 2;
    private static int ScreenIniCenterY => (int)ScreenIniSize.Height / 2;
    private static Brush UserBrush =>
        new SolidColorBrush(Color.FromRgb(Settings.UserColor.R, Settings.UserColor.G, Settings.UserColor.B));
    private static float XAngleSize => Settings.XMaxAngle * 2;
    private static float YAngleSize => Settings.YMaxAngle * 2;
    private static int TargetHp => Settings.TargetHp;

    public static User GetIniUser(string userImagePath, Canvas parent)
    {
        if (File.Exists(userImagePath))
        {
            return new UserPicture
            (
                filePath: userImagePath, 
                center: new(ScreenIniCenterX, ScreenIniCenterY), 
                speed: 0, 
                imageSize: new(Settings.IniUserRadius * 10, Settings.IniUserRadius * 10),
                parent, 
                iniSize: ScreenIniSize
            );
        }
        return new User
        (
            center: new(ScreenIniCenterX, ScreenIniCenterY),
            radius: Settings.IniUserRadius, 
            speed: 0, 
            color: UserBrush, 
            parent, 
            iniSize: ScreenIniSize
        );
    }

    public static ITarget GetIniTarget(string targetImagePath, Point2D<int> center, Canvas parent)
    {
        if (File.Exists(targetImagePath))
        {
            return new TargetPicture
            (
                imageFilePath: targetImagePath,
                center,
                imageSize: new(Settings.IniTargetRadius * 10, Settings.IniTargetRadius * 10),
                parent,
                iniSize: ScreenIniSize
            );
        }
        return new ProgressTarget(center, radius: Settings.IniTargetRadius, parent, TargetHp, iniSize: ScreenIniSize);
    }

    public static Converter GetIniConverter()
    {
        return new(screenSize: ScreenIniSize, angleSize: new(XAngleSize, YAngleSize));
    }
}
