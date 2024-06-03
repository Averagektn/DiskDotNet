using System.Windows;
using System.Windows.Controls;

namespace Disk.View
{
    /// <summary>
    /// Interaction logic for AuthenticationView.xaml
    /// </summary>
    public partial class AuthenticationView : UserControl
    {
        public AuthenticationView()
        {
            InitializeComponent();
            Application.Current.MainWindow.Width = 600;
            Application.Current.MainWindow.Height = 600;    
        }
    }
}
