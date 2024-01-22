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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            var paintWindow = new PaintWindow
            {
                Width = RenderSize.Width,
                Height = RenderSize.Height,
                WindowState = WindowState
            };

            paintWindow.ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSettingsClick(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCalibrationClick(object sender, RoutedEventArgs e)
        {
            new CalibrationWindow().ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnQuitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
