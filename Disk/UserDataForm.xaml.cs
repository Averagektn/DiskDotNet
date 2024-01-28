using System.Windows;

namespace Disk
{
    /// <summary>
    ///     Interaction logic for UserDataForm.xaml
    /// </summary>
    public partial class UserDataForm : Window
    {
        public UserDataForm()
        {
            InitializeComponent();
        }

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            Hide();
            new PaintWindow().ShowDialog();
            Close();
        }
    }
}
