using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Visual.Impl;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Settings = Disk.Properties.Config.Config;

namespace Disk.Service.Implementation
{
    public static class DrawableFabric
    {
        private static Settings Settings => Settings.Default;
        private static readonly Size ScreenIniSize = new(Settings.IniScreenWidth, Settings.IniScreenHeight);
        private static readonly int ScreenIniCenterX = (int)ScreenIniSize.Width / 2;
        private static readonly int ScreenIniCenterY = (int)ScreenIniSize.Height / 2;
        private static readonly Brush UserBrush = new SolidColorBrush(Color.FromRgb(Settings.UserColor.R, Settings.UserColor.G, Settings.UserColor.B));
        private static readonly float XAngleSize = Settings.XMaxAngle * 2;
        private static readonly float YAngleSize = Settings.YMaxAngle * 2;
        private static readonly int TargetHp = Settings.TargetHp;

        public static User GetIniUser(string userImagePath) =>
            File.Exists(userImagePath)
                ? new UserPicture(userImagePath, new(ScreenIniCenterX, ScreenIniCenterY), 0, new(50, 50), ScreenIniSize)
                : new User(new(ScreenIniCenterX, ScreenIniCenterY), Settings.IniUserRadius, 0, UserBrush, ScreenIniSize);

        public static ProgressTarget GetIniProgressTarget(Point2D<int> center) => new(center, Settings.IniTargetRadius, ScreenIniSize, TargetHp);

        public static Converter GetIniConverter() => new(ScreenIniSize, new(XAngleSize, YAngleSize));
    }
}
