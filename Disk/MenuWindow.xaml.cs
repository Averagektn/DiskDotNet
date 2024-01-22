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
        public MenuWindow()
        {
            InitializeComponent();
        }

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            var paintWindow = new PaintWindow
            {
                Width = Width,
                Height = Height
            };

            paintWindow.ShowDialog();
        }

        private void OnSettingsClick(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }

        private void OnCalibrationClick(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }

        private void OnQuitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
