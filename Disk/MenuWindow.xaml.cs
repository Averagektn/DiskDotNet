using System.IO;
using System.Windows;

using Settings = Disk.Properties.Config.Config;

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

            if (!Directory.Exists(Settings.MAIN_DIR_PATH))
            {
                Directory.CreateDirectory(Settings.MAIN_DIR_PATH);
            }

            if (!Directory.Exists(Settings.MAPS_DIR_PATH))
            {
                Directory.CreateDirectory(Settings.MAPS_DIR_PATH);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMapContructorClick(object sender, RoutedEventArgs e)
        {
            Hide();
            new MapCreator().ShowDialog();
            Show();
        }

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            Hide();
            new UserDataForm().ShowDialog();
            Show();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSettingsClick(object sender, RoutedEventArgs e)
        {
            Hide();
            new SettingsWindow().ShowDialog();
            Show();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCalibrationClick(object sender, RoutedEventArgs e)
        {
            Hide();
            new CalibrationWindow().ShowDialog();
            Show();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnQuitClick(object sender, RoutedEventArgs e) => Close();
    }
}
