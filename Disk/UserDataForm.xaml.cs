using System.IO;
using System.Windows;

using Settings = Disk.Config.Config;

namespace Disk
{
    /// <summary>
    ///     Interaction logic for UserDataForm.xaml
    /// </summary>
    public partial class UserDataForm : Window
    {
        private static Settings Settings => Settings.Default;

        public UserDataForm()
        {
            InitializeComponent();
        }

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            Hide();
            new PaintWindow(){ CurrPath = 
                $"{Settings.MAIN_DIR_PATH}{Path.DirectorySeparatorChar}" +
                $"{TbSurname.Text} {TbName.Text}{Path.DirectorySeparatorChar}" +
                $"{DateTime.Now:dd.MM.yyyy HH-mm-ss}" }
            .ShowDialog();
            Close();
        }
    }
}
