using Disk.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Disk.View
{
    /// <summary>
    ///     Interaction logic for AuthenticationView.xaml
    /// </summary>
    public partial class AuthenticationView : UserControl
    {
        public AuthenticationView()
        {
            InitializeComponent();
            Application.Current.MainWindow.Width = 600;
            Application.Current.MainWindow.Height = 600;
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (AuthenticationViewModel)DataContext;
            viewModel.Doctor.Password = PasswordBox.Password;
            viewModel.AuthorizationCommand.Execute(null);
        }

        private void RegistrationBtn_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (AuthenticationViewModel)DataContext;
            viewModel.Doctor.Password = PasswordBox.Password;
            viewModel.RegistrationCommand.Execute(null);
        }
    }
}
