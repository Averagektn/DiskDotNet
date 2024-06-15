using Disk.Calculations.Impl.Converters;
using Disk.Data.Impl;
using Disk.Visual.Impl;
using System.Windows;
using System.Windows.Media;
using Settings = Disk.Properties.Config.Config;

namespace Disk.Service.Implementation
{
    public static class DrawableFabric
    {
        private static Settings Settings => Settings.Default;
        private static readonly Size ScreenIniSize = new(Settings.SCREEN_INI_WIDTH, Settings.SCREEN_INI_HEIGHT);
        private static readonly int ScreenIniCenterX = (int)ScreenIniSize.Width / 2;
        private static readonly int ScreenIniCenterY = (int)ScreenIniSize.Height / 2;
        private static readonly Brush UserBrush =
            new SolidColorBrush(Color.FromRgb(Settings.USER_COLOR.R, Settings.USER_COLOR.G, Settings.USER_COLOR.B));
        private static readonly float XAngleSize = Settings.X_MAX_ANGLE * 2;
        private static readonly float YAngleSize = Settings.Y_MAX_ANGLE * 2;
        private static readonly int TargetHp = 200; // to settings

        public static User GetUser(string userImagePath) =>
            userImagePath != string.Empty
                ? new UserPicture(userImagePath, new(ScreenIniCenterX, ScreenIniCenterY), Settings.USER_INI_SPEED,
                    new(50, 50), ScreenIniSize)
                : new User(new(ScreenIniCenterX, ScreenIniCenterY), Settings.USER_INI_RADIUS, Settings.USER_INI_SPEED,
                    UserBrush, ScreenIniSize);

        public static ProgressTarget GetProgressTarget(Point2D<int> center) =>
            new(center, Settings.TARGET_INI_RADIUS + 5, ScreenIniSize, TargetHp);

        public static Converter GetConverter() => new(ScreenIniSize, new(XAngleSize, YAngleSize));
    }
}
