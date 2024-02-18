using System.IO;
using System.Windows;
using System.Windows.Controls;
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

        private void ChangeLanguage_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var selectedLanguage = menuItem.Tag.ToString();

            Settings.LANGUAGE = selectedLanguage;
            Settings.Save();

            RestartApplication();
        }

        private static void RestartApplication()
        {
            var appPath = Environment.ProcessPath;

            if (appPath is not null)
            {
                System.Diagnostics.Process.Start(appPath);
            }

            Application.Current.Shutdown();
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
