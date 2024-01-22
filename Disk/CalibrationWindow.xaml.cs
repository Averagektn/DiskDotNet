using System.Windows;

namespace Disk
{
    /// <summary>
    ///     Interaction logic for CalibrationWindow.xaml
    /// </summary>
    public partial class CalibrationWindow : Window
    {
        public CalibrationWindow()
        {
            InitializeComponent();

            TbXCoord.Text = Config.Config.Default.X_MAX_ANGLE.ToString();
            TbXCoord.Text = Config.Config.Default.Y_MAX_ANGLE.ToString();
        }

        private void OnCalibrateXClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnCalibrateYClick(object sender, RoutedEventArgs e)
        {

        }
        
        private void OnStartCalibrationClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnApplyClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
