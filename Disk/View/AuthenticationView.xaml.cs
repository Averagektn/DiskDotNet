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
