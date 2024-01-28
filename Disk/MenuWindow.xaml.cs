using System.Windows;

namespace Disk
{
    /// <summary>
    ///     Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public MenuWindow() => InitializeComponent();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
