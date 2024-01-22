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
                Width = 400,
                Height = 300
            };

            paintWindow.ShowDialog();
        }

        private void OnSettingsClick(object sender, RoutedEventArgs e)
        {
 
        }

        private void OnCalibrationClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void OnQuitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
