using System.Windows;
using Settings = Disk.Config.Config;

namespace Disk
{
    /// <summary>
    ///     Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        private static Settings Settings => Settings.Default;

        /// <summary>
        /// 
        /// </summary>
        public MenuWindow()
        {
            InitializeComponent();

            Settings.ANGLE_X_SHIFT = 0;
            Settings.ANGLE_Y_SHIFT = 0;

            Settings.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartClick(object sender, RoutedEventArgs e)
            => new PaintWindow
            {
                Width = Settings.SCREEN_INI_WIDTH,
                Height = Settings.SCREEN_INI_HEIGHT,
                WindowState = WindowState
            }.ShowDialog();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSettingsClick(object sender, RoutedEventArgs e) => new SettingsWindow().ShowDialog();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCalibrationClick(object sender, RoutedEventArgs e) => new CalibrationWindow().ShowDialog();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnQuitClick(object sender, RoutedEventArgs e) => Close();
    }
}
